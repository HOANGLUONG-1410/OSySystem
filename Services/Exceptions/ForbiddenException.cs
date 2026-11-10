using System;

namespace Services.Exceptions
{
    /// <summary>
    /// Lỗi phân quyền (403 Forbidden).
    /// Được ném khi người dùng đã xác thực nhưng không có quyền thực hiện thao tác.
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message)
            : base(message)
        {
        }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
