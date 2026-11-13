using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Exceptions;

namespace Web_API.Middlewares
{
    /// <summary>
    /// Global Exception Middleware bắt toàn bộ unhandled exception từ pipeline.
    /// Chuyển exception thành HTTP response an toàn, không lộ thông tin nhạy cảm.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Cho phép các middleware khác xử lý request.
                // Nếu có exception, nó sẽ được catch ở dòng catch phía dưới.
                await _next(context);
            }
            catch (Exception ex)
            {
                // Kiểm tra xem response đã được gửi đi chưa.
                // Nếu có, không thể thay đổi status code hoặc body.
                if (context.Response.HasStarted)
                {
                    _logger.LogError(
                        ex,
                        "An exception occurred after response had started.");
                    throw;
                }

                // Xử lý exception và tạo error response.
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var response = context.Response;

            // Đặt content type là JSON để client biết parse JSON.
            response.ContentType = "application/json";

            // Tạo error response mặc định (500).
            var errorResponse = new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An unexpected error occurred.",
                Path = context.Request.Path.ToString(),
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            // Mapping từng exception type sang HTTP status code.
            switch (exception)
            {
                case AuthenticationException authEx:
                    // 401 Unauthorized: người dùng chưa xác thực
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = authEx.Message;
                    break;

                case ForbiddenException forbidEx:
                    // 403 Forbidden: người dùng không có quyền
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = forbidEx.Message;
                    break;

                case NotFoundException notFoundEx:
                    // 404 Not Found: tài nguyên không tồn tại
                    response.StatusCode = StatusCodes.Status404NotFound;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = notFoundEx.Message;
                    break;

                case BusinessException bizEx:
                    // 400 Bad Request: vi phạm business rule
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = bizEx.Message;
                    break;

                case ArgumentException argEx:
                    // 400 Bad Request: argument không hợp lệ
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = argEx.Message;
                    break;

                case DbUpdateException dbEx:
                    // 409 Conflict: xung đột database (primary key, unique constraint)
                    response.StatusCode = StatusCodes.Status409Conflict;
                    errorResponse.StatusCode = response.StatusCode;
                    
                    // Không trả chi tiết lỗi SQL
                    if (dbEx.InnerException?.Message.Contains(
                            "Duplicate",
                            StringComparison.OrdinalIgnoreCase) == true)
                    {
                        errorResponse.Message =
                            "A record with this value already exists.";
                    }
                    else
                    {
                        errorResponse.Message =
                            "A database error occurred.";
                    }
                    break;

                default:
                    // 500 Internal Server Error: lỗi hệ thống không dự đoán
                    response.StatusCode =
                        StatusCodes.Status500InternalServerError;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message =
                        "An unexpected error occurred. Please contact support.";
                    break;
            }

            var acceptHeader = context.Request.Headers.Accept.ToString();
            var wantsXml =
                (acceptHeader.Contains("application/xml", StringComparison.OrdinalIgnoreCase)
                 || acceptHeader.Contains("text/xml", StringComparison.OrdinalIgnoreCase))
                && !acceptHeader.Contains("application/json", StringComparison.OrdinalIgnoreCase);

            if (wantsXml)
            {
                response.ContentType = "application/xml; charset=utf-8";
                var serializer = new XmlSerializer(typeof(ErrorResponse));
                using var stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, errorResponse);
                return response.WriteAsync(stringWriter.ToString());
            }

            // Serialize error response thành JSON.
            response.ContentType = "application/json; charset=utf-8";
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonString = JsonSerializer.Serialize(
                errorResponse,
                jsonOptions);

            return response.WriteAsync(jsonString);
        }
    }
}
