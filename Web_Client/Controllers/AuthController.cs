using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers
{
    public class AuthController : Controller
    {
        // Trang đăng nhập. JWT được lưu ở localStorage để phục vụ test API trực quan.
        public IActionResult Login()
        {
            return View();
        }
    }
}
