using System;

namespace Services.Exceptions
{
    /// <summary>
    /// Lỗi xác thực (401 Unauthorized).
    /// Được ném khi người dùng chưa đăng nhập hoặc token không hợp lệ.
    /// </summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message)
            : base(message)
        {
        }

        public AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
