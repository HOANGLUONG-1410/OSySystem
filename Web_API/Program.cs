using System.Security.Claims;
using System.Text;
using BusinessObjects.Data;
using DataAccessObjects;
using DataAccessObjects.DAOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.DTOs.Common;
using Services.Implementations;
using Services.Interfaces;
using Services.Mappings;
using Services.SettingsJWT;
using Web_API.Services;
using Web_API.Middlewares;



namespace Web_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================================================
            // 1. ĐĂNG KÝ CONTROLLER + VALIDATORS
            // =========================================================

            /*
             * AddControllers đăng ký hệ thống Web API Controller.
             * Nếu thiếu dòng này, các Controller sẽ không hoạt động.
             */
            builder.Services.AddScoped<Web_API.Validators.AuthValidator>();

            // [YÊU CẦU: CONTENT NEGOTIATION & MEDIA FORMATTER]
            // - RespectBrowserAcceptHeader: server phản hồi theo header Accept (json hoặc xml)
            // - AddXmlSerializerFormatters: đăng ký Media Formatter hỗ trợ serialization XML
            // [YÊU CẦU: THƯ VIỆN ODATA]
            // - AddOData: kích hoạt các toán tử lọc và phân trang nâng cao
            builder.Services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            })
                .AddOData(options =>
                {
                    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100);
                })
                .AddXmlSerializerFormatters()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(ms => ms.Value?.Errors.Count > 0)
                            .SelectMany(ms => ms.Value!.Errors
                                .Select(e => e.ErrorMessage))
                            .ToList();

                        var friendly = errors.Select(e => MapValidationError(e)).ToList();

                        var message = friendly.Count > 0
                            ? string.Join("; ", friendly)
                            : "Dữ liệu không hợp lệ.";

                        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                            new ApiMessageResponse(message));
                    };
                });

            /*
             * Hỗ trợ Swagger khám phá các endpoint API.
             */
            builder.Services.AddEndpointsApiExplorer();

            // =========================================================
            // 2. CẤU HÌNH DATABASE
            // =========================================================

            /*
             * Đăng ký BunBoDbContext với vòng đời Scoped.
             *
             * Mỗi HTTP request sẽ sử dụng một DbContext.
             * Tất cả DAO, Repository và Service trong cùng request
             * sẽ dùng chung DbContext này.
             */
            builder.Services.AddDbContext<BunBoDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            // =========================================================
            // 3. CẤU HÌNH JWT SETTINGS
            // =========================================================

            /*
             * Đọc section JwtSettings từ appsettings.json.
             *
             * Ví dụ:
             *
             * "JwtSettings": {
             *   "SecretKey": "...",
             *   "Issuer": "...",
             *   "Audience": "...",
             *   "ExpirationMinutes": 120
             * }
             */
            var jwtSettings = builder.Configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>()
                ?? throw new InvalidOperationException(
                    "JwtSettings configuration is missing.");

            /*
             * Đăng ký JwtSettings bằng Options Pattern.
             *
             * Sau đó AuthService có thể inject:
             * IOptions<JwtSettings>
             */
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            // =========================================================
            // 4. CẤU HÌNH JWT AUTHENTICATION
            // =========================================================

            /*
             * Chuyển SecretKey thành mảng byte để tạo khóa ký JWT.
             */
            var secretKey = Encoding.UTF8.GetBytes(
                jwtSettings.SecretKey);

            builder.Services
                .AddAuthentication(options =>
                {
                    /*
                     * Khi endpoint có [Authorize],
                     * ASP.NET Core mặc định sử dụng JWT Bearer
                     * để xác thực request.
                     */
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    /*
                     * Không lưu lại token sau khi xác thực.
                     * API chỉ cần đọc claim từ HttpContext.User.
                     */
                    options.SaveToken = false;

                    /*
                     * Trong môi trường Development có thể để false
                     * nếu API đang chạy HTTP.
                     *
                     * Khi deploy production bằng HTTPS nên để true.
                     */
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            /*
                             * Kiểm tra Issuer:
                             * Token có đúng do hệ thống này phát hành không.
                             */
                            ValidateIssuer = true,
                            ValidIssuer = jwtSettings.Issuer,

                            /*
                             * Kiểm tra Audience:
                             * Token có dành cho client của hệ thống không.
                             */
                            ValidateAudience = true,
                            ValidAudience = jwtSettings.Audience,

                            /*
                             * Kiểm tra thời gian hết hạn của token.
                             */
                            ValidateLifetime = true,

                            /*
                             * Kiểm tra chữ ký JWT.
                             * Nếu token bị sửa nội dung, chữ ký sẽ không hợp lệ.
                             */
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(secretKey),

                            /*
                             * Mặc định Microsoft cho phép token quá hạn
                             * thêm khoảng 5 phút.
                             *
                             * TimeSpan.Zero làm token hết hạn chính xác
                             * theo thời gian đã cấu hình.
                             */
                            ClockSkew = TimeSpan.Zero,

                            /*
                             * ClaimTypes.Role được dùng bởi:
                             *
                             * [Authorize(Roles = "SystemAdmin")]
                             */
                            RoleClaimType = ClaimTypes.Role,

                            /*
                             * ClaimTypes.Name được dùng bởi:
                             *
                             * User.Identity.Name
                             */
                            NameClaimType = ClaimTypes.Name
                        };
                });

            /*
             * Đăng ký hệ thống Authorization.
             *
             * Sau này có thể bổ sung Policy ở đây.
             */
            builder.Services.AddAuthorization();

            // =========================================================
            // 5. ĐĂNG KÝ HTTP CONTEXT
            // =========================================================

            /*
             * Cho phép CurrentUserService đọc:
             *
             * HttpContext.User
             * AccountId claim
             * KitchenId claim
             * Role claim
             */
            builder.Services.AddHttpContextAccessor();

            // =========================================================
            // 6. ĐĂNG KÝ DAO
            // =========================================================

            /*
             * DAO chịu trách nhiệm truy vấn trực tiếp DbContext.
             * DAO không chứa nghiệp vụ phức tạp.
             */

            // Truy vấn bếp tổng và chi nhánh
            builder.Services.AddScoped<KitchenDAO>();

            // Truy vấn danh mục nguyên liệu
            builder.Services.AddScoped<IngredientDAO>();

            // Truy vấn danh mục món ăn
            builder.Services.AddScoped<MenuItemDAO>();

            // Truy vấn công thức món ăn
            builder.Services.AddScoped<RecipeDAO>();

            // Truy vấn số lượng tồn kho hiện tại
            builder.Services.AddScoped<InventoryDAO>();

            // Truy vấn lịch sử biến động kho
            builder.Services.AddScoped<InventoryLogDAO>();

            // Truy vấn đơn hàng
            builder.Services.AddScoped<OrderDAO>();

            // Truy vấn chi tiết món trong đơn
            builder.Services.AddScoped<OrderDetailDAO>();

            // Truy vấn phiếu điều chỉnh tồn kho
            builder.Services.AddScoped<AdjustmentDAO>();

            // Truy vấn phiếu điều chuyển
            builder.Services.AddScoped<TransferRequestDAO>();

            // Truy vấn chi tiết nguyên liệu điều chuyển
            builder.Services.AddScoped<TransferRequestDetailDAO>();

            // Truy vấn tài khoản đăng nhập
            builder.Services.AddScoped<SystemAccountDAO>();

            // Truy vấn lịch làm việc
            builder.Services.AddScoped<ScheduleDAO>();

            // Truy vấn log lịch làm việc
            builder.Services.AddScoped<ScheduleLogDAO>();

            // =========================================================
            // 7. ĐĂNG KÝ REPOSITORY
            // =========================================================

            /*
             * Repository che giấu DAO khỏi tầng Service.
             *
             * Service chỉ làm việc với interface Repository,
             * không gọi DAO trực tiếp.
             */

            builder.Services.AddScoped<
                IKitchenRepository,
                KitchenRepository>();

            builder.Services.AddScoped<
                IIngredientRepository,
                IngredientRepository>();

            builder.Services.AddScoped<
                IMenuItemRepository,
                MenuItemRepository>();

            builder.Services.AddScoped<
                IRecipeRepository,
                RecipeRepository>();

            builder.Services.AddScoped<
                IInventoryRepository,
                InventoryRepository>();

            builder.Services.AddScoped<
                IInventoryLogRepository,
                InventoryLogRepository>();

            builder.Services.AddScoped<
                IOrderRepository,
                OrderRepository>();

            builder.Services.AddScoped<
                IOrderDetailRepository,
                OrderDetailRepository>();

            builder.Services.AddScoped<
                IAdjustmentRepository,
                AdjustmentRepository>();

            builder.Services.AddScoped<
                ITransferRequestRepository,
                TransferRequestRepository>();

            builder.Services.AddScoped<
                ITransferRequestDetailRepository,
                TransferRequestDetailRepository>();

            builder.Services.AddScoped<
                ISystemAccountRepository,
                SystemAccountRepository>();

            builder.Services.AddScoped<
                IScheduleRepository,
                ScheduleRepository>();

            builder.Services.AddScoped<
                IScheduleLogRepository,
                ScheduleLogRepository>();

            // =========================================================
            // 8. ĐĂNG KÝ UNIT OF WORK
            // =========================================================

            /*
             * UnitOfWork quản lý:
             *
             * SaveChangesAsync
             * BeginTransactionAsync
             *
             * Quan trọng:
             * Chỉ giữ một UnitOfWork tại tầng Repositories.
             * Không nên đồng thời giữ UnitOfWork cũ trong DataAccessObjects.
             */
            builder.Services.AddScoped<
                IUnitOfWork,
                UnitOfWork>();

            // =========================================================
            // 9. ĐĂNG KÝ SERVICES
            // =========================================================

            /*
             * Services chứa toàn bộ business logic.
             */

            // Đăng nhập, kiểm tra mật khẩu và sinh JWT
            builder.Services.AddScoped<
                IAuthService,
                AuthService>();

            // Quản lý tài khoản, khóa/mở khóa, đổi mật khẩu
            builder.Services.AddScoped<
                IAccountService,
                AccountService>();

            // Quản lý bếp tổng và chi nhánh
            builder.Services.AddScoped<
                IKitchenService,
                KitchenService>();

            // Quản lý nguyên liệu
            builder.Services.AddScoped<
                IIngredientService,
                IngredientService>();

            // Quản lý món ăn và giá bán
            builder.Services.AddScoped<
                IMenuItemService,
                MenuItemService>();

            // Quản lý công thức món ăn
            builder.Services.AddScoped<
                IRecipeService,
                RecipeService>();

            // Quản lý tồn kho, nhập kho và lịch sử kho
            builder.Services.AddScoped<
                IInventoryService,
                InventoryService>();

            // Quản lý kiểm kê và điều chỉnh kho
            builder.Services.AddScoped<
                IAdjustmentService,
                AdjustmentService>();

            // Quản lý đơn hàng và ConfirmOrder
            builder.Services.AddScoped<
                IOrderService,
                OrderService>();

            // Quản lý điều chuyển nguyên liệu
            builder.Services.AddScoped<
                ITransferService,
                TransferService>();

            // Quản lý lịch làm việc
            builder.Services.AddScoped<
                IScheduleService,
                ScheduleService>();

            /*
             * CurrentUserService đọc thông tin người dùng từ JWT.
             *
             * Class triển khai đặt trong Web_API vì phụ thuộc HttpContext.
             */
            builder.Services.AddScoped<
                ICurrentUserService,
                CurrentUserService>();

            // [YÊU CẦU: THƯ VIỆN AUTOMAPPER]
            // Tự động quét và đăng ký tất cả Profile ánh xạ (Entity <-> DTO) trong assembly Services
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // =========================================================
            // 10. CẤU HÌNH SWAGGER VỚI JWT
            // =========================================================

            builder.Services.AddSwaggerGen(options =>
            {
                /*
                 * Định nghĩa cơ chế Bearer Authentication
                 * trong giao diện Swagger.
                 */
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",

                        Type = SecuritySchemeType.Http,

                        Scheme = "Bearer",

                        BearerFormat = "JWT",

                        In = ParameterLocation.Header,

                        Description =
                            "Enter the JWT access token. " +
                            "Do not add quotation marks."
                    });

                /*
                 * Áp dụng Bearer Security cho các endpoint.
                 *
                 * Sau khi login:
                 * 1. Copy accessToken.
                 * 2. Nhấn Authorize.
                 * 3. Dán token.
                 */
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference =
                                    new OpenApiReference
                                    {
                                        Type =
                                            ReferenceType.SecurityScheme,

                                        Id = "Bearer"
                                    }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            // =========================================================
            // 11. CORS
            // =========================================================

            /*
             * Cho phép Web Client hoặc Flutter Client gọi Web API.
             *
             * Với bài demo, AllowAnyOrigin giúp cấu hình đơn giản.
             * Khi deploy thực tế nên giới hạn domain cụ thể.
             */
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowClient",
                    policy =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // =========================================================
            // 12. MIDDLEWARE PIPELINE
            // =========================================================

            /*
             * Swagger thường chỉ bật trong Development.
             */
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            /*
             * Chuyển HTTP sang HTTPS.
             */
            app.UseHttpsRedirection();

            /*
             * CORS cần được gọi trước Authentication/Authorization.
             */
            app.UseCors("AllowClient");

            /*
             * Global Exception Middleware bắt toàn bộ unhandled exception.
             * Phải đặt đầu pipeline để catch lỗi từ các middleware phía dưới.
             */
            app.UseMiddleware<ExceptionMiddleware>();

            /*
             * Thứ tự bắt buộc:
             *
             * Authentication:
             * Kiểm tra token và xây dựng HttpContext.User.
             *
             * Authorization:
             * Kiểm tra role và quyền của HttpContext.User.
             */
            app.UseAuthentication();
            app.UseAuthorization();

            /*
             * Kết nối route HTTP với Controller.
             */
            app.MapControllers();

            // =========================================================
            // 13. DATABASE MIGRATION & SEEDER
            // =========================================================

            /*
             * Tự động áp dụng Migration (tạo Database nếu chưa có)
             * và tạo tài khoản SystemAdmin mặc định.
             */
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BunBoDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            await DatabaseSeeder.DatabaseSeeder.SeedAdminAsync(app.Services);

            /*
             * Khởi động Web API.
             */
            await app.RunAsync();
        }

        private static string MapValidationError(string error)
        {
            if (error.Contains("Password", StringComparison.OrdinalIgnoreCase)
                && error.Contains("minimum length", StringComparison.OrdinalIgnoreCase))
                return "Mật khẩu phải có ít nhất 6 ký tự.";

            if (error.Contains("Password", StringComparison.OrdinalIgnoreCase)
                && error.Contains("required", StringComparison.OrdinalIgnoreCase))
                return "Mật khẩu không được để trống.";

            if (error.Contains("Username", StringComparison.OrdinalIgnoreCase)
                && error.Contains("required", StringComparison.OrdinalIgnoreCase))
                return "Tên đăng nhập không được để trống.";

            if (error.Contains("FullName", StringComparison.OrdinalIgnoreCase)
                && (error.Contains("required", StringComparison.OrdinalIgnoreCase)
                    || error.Contains("empty", StringComparison.OrdinalIgnoreCase)))
                return "Họ tên không được để trống.";

            if (error.Contains("AccountRole", StringComparison.OrdinalIgnoreCase)
                || error.Contains("enum", StringComparison.OrdinalIgnoreCase))
                return "Vai trò không hợp lệ.";

            if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                return "Tên đăng nhập đã tồn tại.";

            if (error.Contains("kitchen", StringComparison.OrdinalIgnoreCase)
                && error.Contains("belong", StringComparison.OrdinalIgnoreCase))
                return "Nhân viên phải thuộc một chi nhánh.";

            if (error.Contains("BranchManager", StringComparison.OrdinalIgnoreCase)
                && error.Contains("only create Staff", StringComparison.OrdinalIgnoreCase))
                return "Quản lý chi nhánh chỉ được tạo tài khoản nhân viên (Staff).";

            // fallback: cắt bỏ phần kỹ thuật, giữ ý chính
            var idx = error.IndexOf("Path", StringComparison.OrdinalIgnoreCase);
            if (idx > 0) error = error[..idx].TrimEnd(',', ' ');

            idx = error.IndexOf("JSON value", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0) return "Giá trị gửi lên không hợp lệ, vui lòng kiểm tra lại.";

            return error;
        }
    }
}
