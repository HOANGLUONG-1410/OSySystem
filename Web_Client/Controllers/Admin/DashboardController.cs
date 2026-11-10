
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Admin
{
    [Route("Admin/Dashboard")]
    
    public class DashboardController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Admin/Dashboard/Index.cshtml");
    }
}
