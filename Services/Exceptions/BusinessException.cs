using System;

namespace Services.Exceptions
{
    /// <summary>
    /// Lỗi nghiệp vụ (400 Bad Request).
    /// Được ném khi dữ liệu không hợp lệ hoặc vi phạm business rule.
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
