
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Admin
{
    [Route("Admin/Orders")]
    
    public class OrdersController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Admin/Orders/Index.cshtml");
    }
}
