using BusinessObjects.Enums;
using Services.DTOs.Auth;

namespace Web_API.Validators
{
    public class AuthValidator
    {
        public ValidationResult ValidateLogin(LoginRequest request)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(request.Username))
                result.Errors.Add("Tên đăng nhập không được để trống.");

            if (string.IsNullOrWhiteSpace(request.Password))
                result.Errors.Add("Mật khẩu không được để trống.");

            if (result.Errors.Count > 0)
                result.IsValid = false;

            return result;
        }

        public ValidationResult ValidateCreateAccount(CreateAccountRequest request)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(request.Username))
                result.Errors.Add("Tên đăng nhập không được để trống.");
            else if (request.Username.Trim().Length > 50)
                result.Errors.Add("Tên đăng nhập không được quá 50 ký tự.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                result.Errors.Add("Họ tên không được để trống.");
            else if (request.FullName.Trim().Length > 100)
                result.Errors.Add("Họ tên không được quá 100 ký tự.");

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.Errors.Add("Mật khẩu không được để trống.");
            }
            else
            {
                if (request.Password.Length < 6)
                    result.Errors.Add("Mật khẩu phải có ít nhất 6 ký tự.");

                if (!request.Password.Any(char.IsUpper))
                    result.Errors.Add("Mật khẩu phải chứa ít nhất 1 chữ hoa (A-Z).");

                if (!request.Password.Any(char.IsLower))
                    result.Errors.Add("Mật khẩu phải chứa ít nhất 1 chữ thường (a-z).");

                if (!request.Password.Any(char.IsDigit))
                    result.Errors.Add("Mật khẩu phải chứa ít nhất 1 chữ số (0-9).");
            }

            if (request.Role != AccountRole.Staff
                && request.Role != AccountRole.KitchenManager
                && request.Role != AccountRole.BranchManager
                && request.Role != AccountRole.SystemAdmin)
                result.Errors.Add("Vai trò không hợp lệ.");

            if (result.Errors.Count > 0)
                result.IsValid = false;

            return result;
        }
    }
}
