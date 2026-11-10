using System.Text.Json;

namespace Web_API.Middlewares
{
    /// <summary>
    /// Định dạng thông nhất cho tất cả error response từ API.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// HTTP status code (400, 401, 403, 404, 500)
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Thông báo lỗi rõ ràng, an toàn (không có stack trace hoặc connection string).
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Đường dẫn API endpoint bị lỗi.
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// Thời gian lỗi xảy ra (UTC).
        /// </summary>
        public string Timestamp { get; set; } = null!;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
