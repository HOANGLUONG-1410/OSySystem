27/7/2026
# Xoá API Console khỏi client — đã có Swagger cho testing API

- Xoá `Controllers/Admin/ApiConsoleController.cs`
- Xoá `Views/Admin/ApiConsole/Index.cshtml`
- Xoá nav link API Console khỏi `_Layout.cshtml` (cả Web_Client và Views gốc)
- Xoá link "Mở API Console" trong Catalog view
- Xoá `Views/ApiConsole/Index.cshtml` (bản cũ)
- Tất cả role đều không còn giao diện API Console

---

26/7/2026 (bổ sung lần 3)
# Hoàn thiện Schedule + Validation hệ thống + Password toggle + Lỗi tiếng Việt

## 1. Schedule Feature (full stack)
### Entities
- `Schedule` (StaffAccountId, WorkDate, ShiftType, KitchenId, Note) + `ScheduleLog` (Action, OldData, NewData, PerformedByAccountId)

### DAOs
- `ScheduleDAO`: GetAll, GetById, GetByKitchenId, GetByStaffAccountId, GetByKitchenAndDateRange (eager load nav properties)
- `ScheduleLogDAO`: GetAll, **GetByIdAsync** (mới thêm), Add

### Repositories
- `IScheduleRepository` / `ScheduleRepository`
- `IScheduleLogRepository` / `ScheduleLogRepository` (thêm `GetByIdAsync`)

### Services
- `IScheduleService` / `ScheduleService`: GetByKitchenId, GetMySchedules, Create, Update, Delete, GetLogs, **GetLogDetailAsync** (mới — parse JSON diff)

### DTOs
- `CreateScheduleRequest`, `UpdateScheduleRequest`, `ScheduleResponse`, `ScheduleLogResponse`
- `ScheduleLogDetailResponse` + `FieldChange` (mới — diff thay vì JSON raw)

### API Endpoints
| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| GET | `/api/schedules/kitchen/{kitchenId}` | BranchManager, Admin | Xem lịch theo chi nhánh |
| GET | `/api/schedules/my` | Staff | Xem lịch cá nhân |
| POST | `/api/schedules` | BranchManager, Admin | Tạo ca làm |
| PUT | `/api/schedules/{id}` | BranchManager, Admin | Sửa ca làm |
| DELETE | `/api/schedules/{id}` | BranchManager, Admin | Xóa ca làm |
| GET | `/api/schedules/logs` | Admin | Danh sách log |
| GET | `/api/schedules/logs/{logId}` | Admin | **Chi tiết log (diff)** |

### DbContext + Migration
- Thêm `DbSet<Schedule>` + `DbSet<ScheduleLog>` + Fluent API config
- Migration `AddScheduleAndScheduleLog` đã apply

### DatabaseSeeder
- Seed 3 mẫu schedule cho Staff (Sáng/Chiều/Tối)

### Client Views
- **BranchManager/Schedule/Index.cshtml**: CRUD + **gộp theo ngày (rowspan)**
- **Staff/Schedule/Index.cshtml**: thay placeholder → hiển thị lịch, **gộp theo ngày**
- **Admin/ScheduleLogs/Index.cshtml**: dùng API `/logs/{logId}` → render diff (cũ/mới) thay vì JSON raw
- **CSS mới**: `.diff-row`, `.diff-field`, `.diff-values`, `.diff-old`, `.diff-new` trong `site.css`

### Sidebar
- Admin: thêm `/Admin/ScheduleLogs`
- BranchManager: thêm `/BranchManager/Schedule`

## 2. Validation hệ thống (`Web_API/Validators/`)
- `ValidationResult.cs` — class kết quả (IsValid, Errors, Message)
- `AuthValidator.cs` — validate login + tạo tk: empty, độ dài, mk phải có chữ hoa/thường/số
- Đăng ký DI trong `Program.cs`
- `AuthController` dùng `AuthValidator` thay vì auto model validation → message luôn tiếng Việt

## 3. Lỗi tiếng Việt (không còn log .NET)
| Chỗ | Trước | Sau |
|-----|-------|-----|
| Login sai mk | `"Username or password is incorrect."` | `"Tên đăng nhập hoặc mật khẩu không đúng."` |
| Tài khoản bị khóa | `"This account has been deactivated."` | `"Tài khoản đã bị vô hiệu hóa."` |
| Username trùng | `"Username already exists."` | `"Tên đăng nhập đã tồn tại."` |
| Thiếu kitchen | `"Staff must belong to a kitchen."` | `"Nhân viên phải thuộc một chi nhánh."` |
| Model binding lỗi | message .NET kỹ thuật | `MapValidationError` → tiếng Việt |
| BranchManager sai role | `"BranchManager only Staff"` | `"Quản lý chi nhánh chỉ được tạo tài khoản nhân viên."` |

## 4. Password toggle (mắt hiện/ẩn)
- `Auth/Login.cshtml`: thêm nút 👁 bên cạnh input password
- `BranchManager/Personnel/Index.cshtml`: tương tự
- CSS `.input-group .btn-outline-secondary` + `.eye-icon`
- JS function `togglePassword(id, btn)` dùng chung

## 5. Cấu hình JSON + Model Validation
- `Program.cs`: thêm `JsonStringEnumConverter` global — client gửi `"Staff"` (string) → server enum OK
- `Program.cs`: `InvalidModelStateResponseFactory` + `MapValidationError` — fallback cho mọi controller

---

26/7/2026 (bổ sung)
# Fix sidebar role visibility + Thêm Personnel cho BranchManager + Schedule placeholder cho Staff

## Fix: Thanh sidebar hiển thị nhầm link admin cho tất cả role
- Admin links thiếu `style="display:none"` → luôn hiện dù role nào
- Đã thêm `style="display:none"` cho tất cả link, JS chỉ show link khớp role

## Thêm BranchManager/Personnel
- Controller: `Controllers/BranchManager/PersonnelController.cs` → `/BranchManager/Personnel`
- View: `Views/BranchManager/Personnel/Index.cshtml`
- JS gọi `GET /api/accounts` → BranchManager lọc theo KitchenId (chỉ xem Staff trong chi nhánh)
- JS gọi `POST /api/auth/accounts` → BranchManager tạo Staff mới (giới hạn role=Staff, kitchenId=chi nhánh)

## Thêm Staff/Schedule (placeholder)
- Controller: `Controllers/Staff/ScheduleController.cs` → `/Staff/Schedule`
- View: `Views/Staff/Schedule/Index.cshtml`
- API lịch chưa có → hiển thị placeholder "đang phát triển"

## Cập nhật API hỗ trợ BranchManager
- `GET /api/accounts`: thêm role BranchManager → lọc theo KitchenId
- `GET /api/accounts/{id}`: thêm role BranchManager → kiểm tra KitchenId
- `POST /api/auth/accounts`: thêm role BranchManager → chỉ tạo Staff trong chi nhánh

## API còn thiếu (cần phát triển sau)
- `GET/POST /api/schedules` — API quản lý lịch làm việc (BranchManager tạo, Staff xem)
- `PATCH /api/accounts/{id}/deactivate` cho BranchManager — hiện chỉ SystemAdmin mới khóa được tài khoản
- `PUT /api/accounts/{id}` cho BranchManager — hiện chỉ SystemAdmin mới cập nhật được

---

26/7/2026
# Sửa lỗi client không hiển thị dữ liệu + Nâng cấp toàn bộ Views + Seed Data

## Fix 1: JS chạy trước khi api-client.js load
- Bọc toàn bộ JS trong `@section Scripts { }` ở 17 views

## Fix 2: Property name mismatch & API route mismatch
- Orders: `orderCode|id` → `orderId`, `createdAt` → `orderDate`
- Transfers: `sourceKitchenName` → `fromKitchenName`, `destinationKitchenName` → `toKitchenName`
- Catalog: `price` → `averageCost`/`sellingPrice`, `/api/menuitems` → `/api/menu-items`, `type` → `isCentralKitchen`
- KitchenManager/Transfers: `POST /approve` → `PATCH /send`
- Fallback URL: `7066` → `7055`

## Fix 3: Nâng cấp Views đầy đủ chức năng (theo bản cũ)
- Dashboard: metric cards, recent orders/transfers, stock warnings
- Inventory: search, kitchen filter, negative-only, import modal, inventory logs
- Orders: search, status filter, create modal, confirm/complete/cancel, detail view
- Transfers: search, status filter, create modal, send/receive/cancel, detail view
- Catalog: tab view cho Kitchen/Ingredient/MenuItem/Recipe

## Fix 4: Seed Data cho tất cả role
- Thêm `DatabaseSeeder.cs`: tạo 4 tài khoản test
- SystemAdmin: `admin` / `Admin@123`
- KitchenManager: `kitchen.mgr` / `Kitchen@123` (gán Kitchen tổng)
- BranchManager: `branch.mgr` / `Branch@123` (gán Chi nhánh)
- Staff: `staff` / `Staff@123` (gán Chi nhánh)

---

25/7/2026
# PHẦN MỀM QUẢN LÝ CHUỖI BÚN BÒ HUẾ (BunBo Management System)

---

26/7/2026
# Restructure Web_Client theo Role-based Areas

## Lý do
- Views/Controllers trước đây tổ chức flat theo feature, không phân biệt role → chỉ phù hợp SystemAdmin
- Hệ thống có 4 role: SystemAdmin, KitchenManager, BranchManager, Staff → mỗi role cần giao diện riêng

## Giải pháp: Role-based subfolders trong Controllers/ + Views/
Mỗi role là một subfolder riêng, controller dùng `[Route]` attribute để chỉ định URL:
```
Web_Client/
├── Controllers/                 # Role-based subfolders
│   ├── Admin/                   # SystemAdmin (0) - Toàn quyền
│   │   ├── DashboardController.cs   [Route("Admin/Dashboard")]
│   │   ├── InventoryController.cs   [Route("Admin/Inventory")]
│   │   ├── OrdersController.cs      [Route("Admin/Orders")]
│   │   ├── TransfersController.cs   [Route("Admin/Transfers")]
│   │   ├── CatalogController.cs     [Route("Admin/Catalog")]
│   │   └── ApiConsoleController.cs  [Route("Admin/ApiConsole")]
│   ├── KitchenManager/           # KitchenManager (1) - Quản lý bếp trung tâm
│   │   ├── DashboardController.cs   [Route("KitchenManager/Dashboard")]
│   │   ├── InventoryController.cs   [Route("KitchenManager/Inventory")]
│   │   ├── TransfersController.cs   [Route("KitchenManager/Transfers")]
│   │   └── CatalogController.cs     [Route("KitchenManager/Catalog")]
│   ├── BranchManager/            # BranchManager (2) - Quản lý chi nhánh
│   │   ├── DashboardController.cs   [Route("BranchManager/Dashboard")]
│   │   ├── PersonnelController.cs   [Route("BranchManager/Personnel")]
│   │   ├── InventoryController.cs   [Route("BranchManager/Inventory")]
│   │   ├── OrdersController.cs      [Route("BranchManager/Orders")]
│   │   └── CatalogController.cs     [Route("BranchManager/Catalog")]
│   ├── Staff/                    # Staff (3) - Nhân viên
│   │   ├── DashboardController.cs   [Route("Staff/Dashboard")]
│   │   ├── OrdersController.cs      [Route("Staff/Orders")]
│   │   ├── ScheduleController.cs    [Route("Staff/Schedule")]
│   │   └── InventoryController.cs   [Route("Staff/Inventory")]
│   ├── AuthController.cs         # Shared (login)
│   └── HomeController.cs         # Shared (error pages)
├── Views/                        # Role-based subfolders
│   ├── Admin/                    # Views cho SystemAdmin
│   ├── KitchenManager/           # Views cho KitchenManager
│   ├── BranchManager/            # Views cho BranchManager
│   └── Staff/                    # Views cho Staff
```


## Software Requirements Specification (SRS)

---

## 1. Tổng quan dự án

**Tên dự án:** BunBo Management System  
**Mã môn học:** PRN232  
**Nền tảng:** .NET 8 (ASP.NET Core Web API + MVC Client)  
**CSDL:** SQL Server (BunBoManagementDb)

Hệ thống quản lý chuỗi nhà hàng Bún Bò Huế, bao gồm bếp trung tâm (central kitchen) và các chi nhánh (branches). Hỗ trợ quản lý nguyên liệu, thực đơn, công thức nấu, tồn kho, đơn hàng, chuyển kho liên bếp và điều chỉnh tồn kho.

---

## 2. Kiến trúc hệ thống

### 2.1 Mô hình layered architecture (6 lớp)

```
┌─────────────────────────────────────────────────┐
│              Web_Client (MVC Client)            │
│     (Razor Views + JS SPA gọi API trực tiếp)    │
├─────────────────────────────────────────────────┤
│              Web_API (REST API)                 │
│  Controllers → Middleware → JWT Auth → Swagger  │
├─────────────────────────────────────────────────┤
│                Services (Business Logic)        │
│  Service Interfaces + Implementations + DTOs    │
├─────────────────────────────────────────────────┤
│              Repositories (Abstraction)         │
│  Interfaces + Implementations + UnitOfWork      │
├─────────────────────────────────────────────────┤
│           DataAccessObjects (Data Access)       │
│  DAOs (truy vấn EF Core trực tiếp)              │
├─────────────────────────────────────────────────┤
│             BusinessObjects (Domain)            │
│  Entities, DbContext, Enums, Migrations         │
└─────────────────────────────────────────────────┘
```

### 2.2 Luồng dữ liệu

```
Client (JS) ──REST API──► Web_API ──► Services ──► Repositories ──► DAOs ──► DbContext ──► SQL Server
                ◄─────────    ◄────────     ◄──────────    ◄───────    ◄───────
```

### 2.3 Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Runtime | .NET 8 |
| ORM | Entity Framework Core 8.0.8 |
| Database | SQL Server |
| Authentication | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer 8.0.8) |
| Password Hashing | BCrypt.Net-Next 4.0.3 |
| API Docs | Swashbuckle 6.7.0 (Swagger) |
| Client | ASP.NET Core MVC + JavaScript (SPA-style) |
| NuGet | System.IdentityModel.Tokens.Jwt 8.1.0 |

---

## 3. Cấu trúc thư mục

```
Project_PRN232_Backup/
├── BusinessObjects/            # Lớp Domain
│   ├── Data/
│   │   └── BunBoDbContext.cs
│   ├── Enums/
│   │   ├── AccountRole.cs          (SystemAdmin, KitchenManager, BranchManager, Staff)
│   │   ├── InventoryLogType.cs     (Import, Export, Adjustment, TransferIn, TransferOut, Waste)
│   │   ├── OrderStatus.cs          (Pending, Processing, Completed, Cancelled)
│   │   └── TransferStatus.cs       (Pending, Approved, Completed, Rejected)
│   ├── Migrations/                 # EF Core Migrations (InitialCreate, AddSystemAccount, AddAuditFields)
│   ├── Models/
│   │   ├── SystemAccount.cs
│   │   ├── Kitchen.cs
│   │   ├── Ingredient.cs
│   │   ├── MenuItem.cs
│   │   ├── Recipe.cs
│   │   ├── Inventory.cs
│   │   ├── InventoryLog.cs
│   │   ├── Order.cs
│   │   ├── OrderDetail.cs
│   │   ├── Adjustment.cs
│   │   ├── TransferRequest.cs
│   │   ├── TransferRequestDetail.cs
│   │   ├── Schedule.cs
│   │   └── ScheduleLog.cs
│   └── BusinessObjects.csproj
├── DataAccessObjects/          # Lớp truy xuất dữ liệu
│   ├── Base/                   (chứa DbContext DI)
│   └── DAOs/
│       ├── AdjustmentDAO.cs
│       ├── IngredientDAO.cs
│       ├── InventoryDAO.cs
│       ├── InventoryLogDAO.cs
│       ├── KitchenDAO.cs
│       ├── MenuItemDAO.cs
│       ├── OrderDAO.cs
│       ├── OrderDetailDAO.cs
│       ├── RecipeDAO.cs
│       ├── SystemAccountDAO.cs
│       ├── TransferRequestDAO.cs
│       ├── TransferRequestDetailDAO.cs
│       ├── ScheduleDAO.cs
│       └── ScheduleLogDAO.cs
│   └── DataAccessObjects.csproj
├── Repositories/               # Lớp Repository (Interface + Implementation)
│   ├── Interfaces/
│   └── Implementations/        (+ IUnitOfWork / UnitOfWork)
│   └── Repositories.csproj
├── Services/                   # Lớp Business Logic
│   ├── DTOs/
│   │   ├── Auth/               (LoginRequest, LoginResponse, CreateAccountRequest)
│   │   ├── Accounts/           (ChangePasswordRequest, ResetPasswordRequest, UpdateAccountRequest, AccountResponse)
│   │   ├── Adjustments/        (CreateAdjustmentRequest, AdjustmentResponse)
│   │   ├── Ingredient/         (CreateIngredientRequest, UpdateIngredientRequest, IngredientResponse)
│   │   ├── Inventories/        (CreateInventoryRequest, ImportInventoryRequest, InventoryResponse, InventoryLogResponse)
│   │   ├── Kitchens/           (CreateKitchenRequest, UpdateKitchenRequest, KitchenResponse)
│   │   ├── MenuItem/           (CreateMenuItemRequest, UpdateMenuItemRequest, MenuItemResponse)
│   │   ├── Orders/             (CreateOrderRequest, CreateOrderDetailRequest, OrderResponse)
│   │   ├── Recipes/            (CreateRecipeRequest, UpdateRecipeRequest, RecipeResponse, RecipeItemResponse)
│   │   ├── Schedules/          (CreateScheduleRequest, UpdateScheduleRequest, ScheduleResponse, ScheduleLogResponse, ScheduleLogDetailResponse, FieldChange)
│   │   └── Transfers/          (CreateTransferRequest, TransferDetailRequest, TransferRequestResponse, TransferDetailResponse, TransferActionResponse)
│   ├── Exceptions/
│   │   ├── AuthenticationException.cs   → 401
│   │   ├── ForbiddenException.cs        → 403
│   │   ├── BusinessException.cs         → 400
│   │   └── NotFoundException.cs         → 404
│   ├── Interfaces/
│   ├── Implementations/
│   └── Services.csproj
├── Web_API/                    # REST API
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── AccountsController.cs
│   │   ├── KitchensController.cs
│   │   ├── IngredientsController.cs
│   │   ├── MenuItemsController.cs
│   │   ├── RecipesController.cs
│   │   ├── InventoriesController.cs
│   │   ├── AdjustmentsController.cs
│   │   ├── OrdersController.cs
│   │   ├── TransfersController.cs
│   │   └── SchedulesController.cs
│   ├── Middlewares/
│   │   ├── ExceptionMiddleware.cs
│   │   └── ErrorResponse.cs
│   ├── Validators/
│   │   ├── ValidationResult.cs
│   │   └── AuthValidator.cs
│   ├── Services/
│   │   └── CurrentUserService.cs
│   ├── DatabaseSeeder/
│   │   └── DatabaseSeeder.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Web_API.csproj
├── Web_Client/                 # MVC Client
│   ├── Controllers/            # Role-based subfolders (26/7/2026)
│   │   ├── Admin/              # SystemAdmin - Toàn quyền
│   │   │   ├── DashboardController.cs   [Route("Admin/Dashboard")]
│   │   │   ├── InventoryController.cs   [Route("Admin/Inventory")]
│   │   │   ├── OrdersController.cs      [Route("Admin/Orders")]
│   │   │   ├── TransfersController.cs   [Route("Admin/Transfers")]
│   │   │   ├── CatalogController.cs     [Route("Admin/Catalog")]
│   │   │   └── ScheduleLogsController.cs [Route("Admin/ScheduleLogs")]
│   │   ├── KitchenManager/     # Quản lý bếp trung tâm
│   │   │   ├── DashboardController.cs   [Route("KitchenManager/Dashboard")]
│   │   │   ├── InventoryController.cs   [Route("KitchenManager/Inventory")]
│   │   │   ├── TransfersController.cs   [Route("KitchenManager/Transfers")]
│   │   │   └── CatalogController.cs     [Route("KitchenManager/Catalog")]
│   │   ├── BranchManager/      # Quản lý chi nhánh
│   │   │   ├── DashboardController.cs   [Route("BranchManager/Dashboard")]
│   │   │   ├── PersonnelController.cs   [Route("BranchManager/Personnel")]
│   │   │   ├── ScheduleController.cs    [Route("BranchManager/Schedule")]
│   │   │   ├── InventoryController.cs   [Route("BranchManager/Inventory")]
│   │   │   ├── OrdersController.cs      [Route("BranchManager/Orders")]
│   │   │   └── CatalogController.cs     [Route("BranchManager/Catalog")]
│   │   ├── Staff/              # Nhân viên
│   │   │   ├── DashboardController.cs   [Route("Staff/Dashboard")]
│   │   │   ├── OrdersController.cs      [Route("Staff/Orders")]
│   │   │   ├── ScheduleController.cs    [Route("Staff/Schedule")]
│   │   │   └── InventoryController.cs   [Route("Staff/Inventory")]
│   │   ├── AuthController.cs   # Login (không yêu cầu role)
│   │   └── HomeController.cs   # Error pages
│   ├── Views/                  # Role-based subfolders
│   │   ├── Admin/
│   │   │   ├── Dashboard/Index.cshtml
│   │   │   ├── Inventory/Index.cshtml
│   │   │   ├── Orders/Index.cshtml
│   │   │   ├── Transfers/Index.cshtml
│   │   │   ├── Catalog/Index.cshtml
│   │   │   └── ScheduleLogs/Index.cshtml
│   │   ├── KitchenManager/
│   │   │   ├── Dashboard/Index.cshtml
│   │   │   ├── Inventory/Index.cshtml
│   │   │   ├── Transfers/Index.cshtml
│   │   │   └── Catalog/Index.cshtml
│   │   ├── BranchManager/
│   │   │   ├── Dashboard/Index.cshtml
│   │   │   ├── Personnel/Index.cshtml
│   │   │   ├── Schedule/Index.cshtml
│   │   │   ├── Inventory/Index.cshtml
│   │   │   ├── Orders/Index.cshtml
│   │   │   └── Catalog/Index.cshtml
│   │   ├── Staff/
│   │   │   ├── Dashboard/Index.cshtml
│   │   │   ├── Orders/Index.cshtml
│   │   │   ├── Schedule/Index.cshtml
│   │   │   └── Inventory/Index.cshtml
│   │   ├── Auth/Login.cshtml
│   │   ├── Home/...
│   │   ├── Shared/_Layout.cshtml
│   │   ├── _ViewStart.cshtml
│   │   └── _ViewImports.cshtml
│   ├── wwwroot/
│   │   ├── js/
│   │   │   ├── api-client.js      (# SPA Engine: OsyApi, OsyAuth, OsyUi)
│   │   │   └── site.js
│   │   └── css/
│   │       └── site.css
│   ├── Models/
│   │   └── ErrorViewModel.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Web_Client.csproj
├── AUDIT_COMPLETION_SUMMARY.md
├── IMPLEMENTATION_ROADMAP.md
├── WEB_CLIENT_GUIDE.md
└── Project_PRN232_Backup.sln
```

---

## 4. Cơ sở dữ liệu

### 4.1 Danh sách bảng

| # | Bảng | Mô tả |
|---|------|-------|
| 1 | **SystemAccount** | Tài khoản người dùng (admin, quản lý bếp, nhân viên) |
| 2 | **Kitchen** | Bếp (trung tâm hoặc chi nhánh) |
| 3 | **Ingredient** | Nguyên liệu |
| 4 | **MenuItem** | Món ăn trong thực đơn |
| 5 | **Recipe** | Công thức (nguyên liệu + định lượng cho mỗi món) |
| 6 | **Inventory** | Tồn kho nguyên liệu theo từng bếp |
| 7 | **InventoryLog** | Lịch sử thay đổi tồn kho |
| 8 | **Order** | Đơn hàng |
| 9 | **OrderDetail** | Chi tiết đơn hàng (món ăn + số lượng) |
| 10 | **TransferRequest** | Yêu cầu chuyển nguyên liệu giữa các bếp |
| 11 | **TransferRequestDetail** | Chi tiết nguyên liệu trong yêu cầu chuyển |
| 12 | **Adjustment** | Điều chỉnh tồn kho (kiểm kê, hao hụt) |
| 13 | **Schedule** | Lịch làm việc ca cho nhân viên |
| 14 | **ScheduleLog** | Lịch sử thay đổi lịch làm việc |

### 4.2 Mối quan hệ chính

```
SystemAccount (1) ──── (N) Kitchen (quản lý)
Kitchen (1) ────────── (N) Inventory
Kitchen (1) ────────── (N) Order
Kitchen (1) ────────── (N) TransferRequest (source/destination)
Ingredient (1) ─────── (N) Inventory
Ingredient (1) ─────── (N) Recipe
MenuItem (1) ───────── (N) Recipe
MenuItem (1) ───────── (N) OrderDetail
Order (1) ──────────── (N) OrderDetail
TransferRequest (1) ── (N) TransferRequestDetail
```

### 4.3 Connection String

```
Server=.;Database=BunBoManagementDb;Uid=sa;Pwd=123;TrustServerCertificate=True
```

---

## 5. Authentication & Authorization

### 5.1 JWT Settings

| Setting | Giá trị |
|---------|---------|
| Secret Key | `BUN_BO_MANAGEMENT_PRN232_SECRET_KEY_2026_CHANGE_ME` |
| Issuer | `BunBoManagement` |
| Audience | `BunBoManagement` |
| Expiry | 120 phút |

### 5.2 Claims trong JWT

| Claim | Mô tả |
|-------|-------|
| `AccountId` | ID tài khoản |
| `Username` | Tên đăng nhập |
| `Role` | Vai trò (enum) |
| `KitchenId` | Bếp phụ trách (nếu có) |

### 5.3 Vai trò (Roles)

| Role | Giá trị | Quyền hạn |
|------|---------|-----------|
| **SystemAdmin** | 0 | Toàn quyền |
| **KitchenManager** | 1 | Quản lý bếp trung tâm |
| **BranchManager** | 2 | Quản lý chi nhánh |
| **Staff** | 3 | Nhập đơn hàng, xem tồn kho |

---

## 6. API Endpoints

### 6.1 Auth
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/auth/login` | Đăng nhập → trả về JWT |
| POST | `/api/auth/register` | Đăng ký (admin) |

### 6.2 Accounts
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/accounts` | Danh sách tài khoản |
| GET | `/api/accounts/{id}` | Chi tiết tài khoản |
| PUT | `/api/accounts/{id}` | Cập nhật tài khoản |
| PUT | `/api/accounts/{id}/change-password` | Đổi mật khẩu |
| PUT | `/api/accounts/{id}/reset-password` | Reset mật khẩu |
| DELETE | `/api/accounts/{id}` | Xóa tài khoản |

### 6.3 Kitchens
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/kitchens` | Danh sách bếp |
| GET | `/api/kitchens/{id}` | Chi tiết bếp |
| POST | `/api/kitchens` | Thêm bếp |
| PUT | `/api/kitchens/{id}` | Cập nhật bếp |
| DELETE | `/api/kitchens/{id}` | Xóa bếp |

### 6.4 Ingredients
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/ingredients` | Danh sách nguyên liệu |
| GET | `/api/ingredients/{id}` | Chi tiết nguyên liệu |
| POST | `/api/ingredients` | Thêm nguyên liệu |
| PUT | `/api/ingredients/{id}` | Cập nhật nguyên liệu |
| DELETE | `/api/ingredients/{id}` | Xóa nguyên liệu |

### 6.5 MenuItems
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/menuitems` | Danh sách món ăn |
| GET | `/api/menuitems/{id}` | Chi tiết món |
| POST | `/api/menuitems` | Thêm món |
| PUT | `/api/menuitems/{id}` | Cập nhật món |
| DELETE | `/api/menuitems/{id}` | Xóa món |

### 6.6 Recipes
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/recipes` | Danh sách công thức |
| GET | `/api/recipes/{id}` | Chi tiết công thức |
| POST | `/api/recipes` | Thêm công thức |
| PUT | `/api/recipes/{id}` | Cập nhật công thức |
| DELETE | `/api/recipes/{id}` | Xóa công thức |

### 6.7 Inventories
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/inventories` | Danh sách tồn kho (theo kitchen) |
| GET | `/api/inventories/{id}` | Chi tiết tồn kho |
| POST | `/api/inventories` | Tạo tồn kho |
| POST | `/api/inventories/import` | Nhập nguyên liệu |
| GET | `/api/inventories/logs` | Lịch sử tồn kho |

### 6.8 Adjustments
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/adjustments` | Danh sách điều chỉnh |
| POST | `/api/adjustments` | Tạo điều chỉnh |

### 6.9 Orders
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/orders` | Danh sách đơn hàng |
| GET | `/api/orders/{id}` | Chi tiết đơn hàng |
| POST | `/api/orders` | Tạo đơn hàng |
| PUT | `/api/orders/{id}/status` | Cập nhật trạng thái |
| DELETE | `/api/orders/{id}` | Xóa đơn hàng |

### 6.10 Transfers
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/transfers` | Danh sách yêu cầu chuyển |
| GET | `/api/transfers/{id}` | Chi tiết yêu cầu |
| POST | `/api/transfers` | Tạo yêu cầu chuyển |
| POST | `/api/transfers/{id}/approve` | Duyệt yêu cầu |
| POST | `/api/transfers/{id}/reject` | Từ chối |

### 6.11 Schedules
| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| GET | `/api/schedules/kitchen/{kitchenId}` | BranchManager, Admin | Xem lịch theo chi nhánh |
| GET | `/api/schedules/my` | Staff | Xem lịch cá nhân |
| POST | `/api/schedules` | BranchManager, Admin | Tạo ca làm |
| PUT | `/api/schedules/{id}` | BranchManager, Admin | Sửa ca làm |
| DELETE | `/api/schedules/{id}` | BranchManager, Admin | Xóa ca làm |
| GET | `/api/schedules/logs` | Admin | Danh sách lịch sử thay đổi |
| GET | `/api/schedules/logs/{logId}` | Admin | Chi tiết thay đổi (diff các field) |

---

## 7. Giao diện Client (Web_Client)

### 7.1 Danh sách trang (theo role - cập nhật 26/7/2026 lần 3)

#### SystemAdmin (`/Admin/...`)
| Route | View | Mô tả |
|-------|------|-------|
| `/Admin/Dashboard` | Views/Admin/Dashboard/Index.cshtml | Bảng tổng quan toàn hệ thống |
| `/Admin/Inventory` | Views/Admin/Inventory/Index.cshtml | Quản lý tồn kho + nhập hàng + lịch sử |
| `/Admin/Orders` | Views/Admin/Orders/Index.cshtml | Quản lý đơn hàng |
| `/Admin/Transfers` | Views/Admin/Transfers/Index.cshtml | Chuyển kho liên bếp |
| `/Admin/Catalog` | Views/Admin/Catalog/Index.cshtml | Quản lý danh mục (bếp, nguyên liệu, món ăn, công thức) - full CRUD |
| `/Admin/ScheduleLogs` | Views/Admin/ScheduleLogs/Index.cshtml | Lịch sử thay đổi lịch làm việc |

#### KitchenManager (`/KitchenManager/...`)
| Route | View | Mô tả |
|-------|------|-------|
| `/KitchenManager/Dashboard` | Views/KitchenManager/Dashboard/Index.cshtml | Tổng quan bếp trung tâm |
| `/KitchenManager/Inventory` | Views/KitchenManager/Inventory/Index.cshtml | Quản lý tồn kho bếp trung tâm |
| `/KitchenManager/Transfers` | Views/KitchenManager/Transfers/Index.cshtml | Tạo/chuyển nguyên liệu đi |
| `/KitchenManager/Catalog` | Views/KitchenManager/Catalog/Index.cshtml | Quản lý nguyên liệu (CRUD), món ăn (read-only), công thức (CRUD) |

#### BranchManager (`/BranchManager/...`)
| Route | View | Mô tả |
|-------|------|-------|
| `/BranchManager/Dashboard` | Views/BranchManager/Dashboard/Index.cshtml | Tổng quan chi nhánh |
| `/BranchManager/Personnel` | Views/BranchManager/Personnel/Index.cshtml | Quản lý nhân sự (xem/tạo Staff) |
| `/BranchManager/Schedule` | Views/BranchManager/Schedule/Index.cshtml | Quản lý lịch làm việc (CRUD) |
| `/BranchManager/Inventory` | Views/BranchManager/Inventory/Index.cshtml | Xem tồn kho chi nhánh |
| `/BranchManager/Orders` | Views/BranchManager/Orders/Index.cshtml | Đặt hàng cho chi nhánh |
| `/BranchManager/Catalog` | Views/BranchManager/Catalog/Index.cshtml | Xem danh mục (read-only) |

#### Staff (`/Staff/...`)
| Route | View | Mô tả |
|-------|------|-------|
| `/Staff/Dashboard` | Views/Staff/Dashboard/Index.cshtml | Dashboard nhân viên |
| `/Staff/Orders` | Views/Staff/Orders/Index.cshtml | Tạo/xem đơn hàng |
| `/Staff/Schedule` | Views/Staff/Schedule/Index.cshtml | Lịch làm việc (xem ca của bản thân) |
| `/Staff/Inventory` | Views/Staff/Inventory/Index.cshtml | Xem tồn kho |

#### Shared (không yêu cầu role)
| Route | View | Mô tả |
|-------|------|-------|
| `/Auth/Login` | Views/Auth/Login.cshtml | Đăng nhập |
| `/Home/Error` | Views/Shared/Error.cshtml | Trang lỗi |

### 7.2 SPA Engine (api-client.js)

Client dùng JavaScript thuần gọi API trực tiếp, gồm 3 module:

| Module | Chức năng |
|--------|-----------|
| **OsyApi** | Wrapper fetch API (GET/POST/PUT/DELETE), tự động đính kèm JWT |
| **OsyAuth** | Đăng nhập, logout, lưu token vào localStorage, giải mã JWT lấy claims |
| **OsyUi** | Render bảng động, modal, form, toast notification |

### 7.3 Layout

- Sidebar: menu điều hướng
- Topbar: tên người dùng + role + nút logout
- Brand theme: màu cam/đỏ (ẩm thực Việt)

---

## 8. Xử lý lỗi (Exception Handling)

| Exception | HTTP Status | Khi nào xảy ra |
|-----------|-------------|----------------|
| `NotFoundException` | 404 | Không tìm thấy entity |
| `AuthenticationException` | 401 | Sai credentials, token hết hạn |
| `ForbiddenException` | 403 | Không có quyền truy cập |
| `BusinessException` | 400 | Nghiệp vụ không hợp lệ (số lượng âm, ...) |

Middleware `ExceptionMiddleware` tự động bắt và trả về JSON format:
```json
{
  "message": "Chi tiết lỗi",
  "statusCode": 400
}
```

---

## 9. Seed Data

`DatabaseSeeder` chạy khi ứng dụng khởi động:

- **SystemAdmin**: username = `admin`, password = `Admin@123`

---

## 10. Cấu hình triển khai

| Thành phần | URL |
|------------|-----|
| Web_API (HTTPS) | `https://localhost:7055` |
| Web_API (HTTP) | `http://localhost:5012` |
| Web_Client (HTTPS) | `https://localhost:7165` |
| Web_Client (HTTP) | `http://localhost:5043` |

Web_Client gọi API qua `ApiSettings:BaseUrl = https://localhost:7055`

---

## 11. Tình trạng hoàn thành (~30%)

Theo audit (`AUDIT_COMPLETION_SUMMARY.md`), các hạng mục cần hoàn thiện:

1. Cập nhật 7 services với custom exceptions chuẩn
2. Thêm `AsNoTracking()` + `Include()` vào 10 DAOs
3. Thêm audit fields vào response DTOs
4. Đơn giản hóa controllers (bỏ try-catch thừa)
5. Thêm pagination DTOs
6. Hoàn thiện DatabaseSeeder với dữ liệu mẫu
7. Bổ sung API documentation

---

## 12. Hướng phát triển (IMPLEMENTATION_ROADMAP.md)

| Phase | Nội dung |
|-------|----------|
| 1 | Hoàn thiện exception handling & validation |
| 2 | Tối ưu truy vấn (AsNoTracking, Include) |
| 3 | Audit fields & response standardization |
| 4 | Clean architecture refactor (bỏ try-catch controllers) |
| 5 | Pagination & filtering |
| 6 | Seed data hoàn chỉnh |
| 7 | API documentation (Swagger XML comments) |
| 8 | Unit test & Integration test |
| 9 | CI/CD & Deployment |

---

## 13. Luồng của dự án

### 13.1 Luồng xác thực (Authentication Flow)

```
Người dùng → Mở Web_Client → /Auth/Login
  → Nhập username/password
  → POST /api/auth/login → Web_API kiểm tra credentials (BCrypt)
  → Thành công → Trả về JWT (AccountId, Role, KitchenId, FullName)
  → Client lưu token vào localStorage (osy_access_token)
  → OsyAuth.getRoleRedirect() → chuyển hướng đến Dashboard theo role
  → Các request sau tự động đính kèm Bearer token qua OsyApi.request()
```

### 13.2 Luồng phân quyền (Authorization Flow)

```
Mỗi request đến API:
  OsyApi.request(path) → lấy token từ localStorage → gửi Authorization: Bearer <token>
  Web_API: [Authorize] kiểm tra JWT hợp lệ
  [Authorize(Roles = "SystemAdmin,KitchenManager")] kiểm tra role claim
  CurrentUserService đọc HttpContext.User → AccountId, Role, KitchenId
  Service layer dùng CurrentUserService để kiểm tra quyền truy cập dữ liệu
```

### 13.3 Luồng quản lý danh mục (Catalog Flow) — cập nhật 27/7/2026

```
Mỗi tab trong Catalog hoạt động độc lập theo pattern:

[Thêm] → Mở modal trống → Nhập dữ liệu → Submit → POST /api/... → Tạo mới
[Sửa] → Mở modal có dữ liệu → Chỉnh sửa → Submit → PUT /api/.../{id} → Cập nhật
[Vô hiệu/Xoá] → OsyUi.confirm() → PATCH /.../deactivate hoặc DELETE → Xoá mềm/cứng

Phân quyền Catalog (27/7/2026):

| Tab | SystemAdmin | KitchenManager | BranchManager |
|-----|:-----------:|:--------------:|:-------------:|
| Bếp | CRUD | - | - |
| Nguyên liệu | CRUD | Thêm/Sửa | Read-only |
| Món ăn | CRUD | Read-only | Read-only |
| Công thức | CRUD | CRUD | - |
```

### 13.4 Luồng xử lý đơn hàng (Order Flow)

```
Staff/BranchManager → Mở /Orders → Nhấn "Tạo đơn"
  → Modal chọn món ăn (MenuItem) + số lượng
  → POST /api/orders → Tạo Order với status = Pending
  → Nhấn "Xác nhận" → PATCH /api/orders/{id}/status → Processing
  → Bếp chế biến → Nhấn "Hoàn thành" → Completed
  → Hoặc "Huỷ" → Cancelled
```

### 13.5 Luồng nhập kho (Inventory Import Flow)

```
Admin/KitchenManager → Mở /Inventory → Nhấn "Nhập kho"
  → Chọn Kitchen, Nguyên liệu, Số lượng, Đơn giá, Ghi chú
  → POST /api/inventories/import
  → Service kiểm tra tồn kho hiện tại:
      - Nếu chưa có Inventory record → Tạo mới
      - Nếu đã có → Cập nhật quantity + averageCost (bình quân gia quyền)
  → Tạo InventoryLog với logType = Import
  → Reload bảng tồn kho
```

### 13.6 Luồng chuyển kho (Transfer Flow)

```
Admin/KitchenManager → Mở /Transfers → Nhấn "Tạo phiếu"
  → Chọn bếp nguồn, bếp đích, nguyên liệu + số lượng
  → POST /api/transfers → Tạo TransferRequest với status = Pending
  → Bếp nguồn gửi đi → PATCH /api/transfers/{id}/send
      → Giảm tồn kho bếp nguồn
      → Chuyển status → InTransit
  → Bếp đích nhận → PATCH /api/transfers/{id}/receive
      → Tăng tồn kho bếp đích
      → Tạo InventoryLog cho cả 2 bếp
      → Chuyển status → Completed
  → Hoặc "Huỷ" → Cancelled
```

### 13.7 Luồng quản lý lịch làm việc (Schedule Flow)

```
BranchManager → Mở /Schedule
  → Xem lịch theo KitchenId → GET /api/schedules/kitchen/{kitchenId}
  → Thêm ca → Modal chọn Staff, Ngày, Ca (Sáng/Chiều/Tối), Ghi chú
      → POST /api/schedules → Tạo Schedule
  → Sửa ca → PUT /api/schedules/{id}
  → Xoá ca → DELETE /api/schedules/{id}
  → Mỗi thay đổi → Tự động ghi ScheduleLog (diff các field cũ/mới)

Staff → Mở /Schedule
  → Xem ca của bản thân → GET /api/schedules/my
  → Hiển thị bảng lịch gộp theo ngày
```

### 13.8 Luồng dữ liệu tổng quát (Data Flow)

```
┌─────────────────────────────────────────────────────────────┐
│                     Web_Client (MVC)                        │
│  Razor View ──(render)── JS (OsyApi)                        │
│       ↑                        ↓                             │
│  osy_user (localStorage)   fetch() với Bearer JWT            │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP REST JSON
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                     Web_API (REST)                           │
│  Controller ← [Authorize] ← ExceptionMiddleware              │
│       ↓                                                      │
│  Service (business logic + validation)                       │
│       ↓                                                      │
│  Repository (abstraction layer)                              │
│       ↓                                                      │
│  DAO (Entity Framework Core queries)                         │
│       ↓                                                      │
│  DbContext ──→ SQL Server                                    │
└─────────────────────────────────────────────────────────────┘
```

### 13.9 Luồng điều hướng (Navigation Flow)

```
Người dùng đăng nhập → OsyAuth.getRoleRedirect():
  SystemAdmin (0)    → /Admin/Dashboard
  KitchenManager (1) → /KitchenManager/Dashboard
  BranchManager (2)  → /BranchManager/Dashboard
  Staff (3)          → /Staff/Dashboard

Sidebar hiển thị động dựa trên role:
  data-roles attribute → JS đọc OsyAuth.getUser().role
  → Chỉ hiển thị link phù hợp với role hiện tại

Mỗi role chỉ thấy feature thuộc quyền:
  SystemAdmin: Dashboard, Inventory, Orders, Transfers, Catalog, ScheduleLogs
  KitchenManager: Dashboard, Inventory, Transfers, Catalog
  BranchManager: Dashboard, Personnel, Schedule, Inventory, Orders, Catalog
  Staff: Dashboard, Orders, Schedule, Inventory
```
