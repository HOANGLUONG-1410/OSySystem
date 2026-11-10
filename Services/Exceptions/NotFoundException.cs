using System;

namespace Services.Exceptions
{
    /// <summary>
    /// Lỗi không tìm thấy (404 Not Found).
    /// Được ném khi tài nguyên không tồn tại trong database.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
