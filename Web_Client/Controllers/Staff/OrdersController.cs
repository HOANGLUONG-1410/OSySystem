
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Staff
{
    [Route("Staff/Orders")]
    
    public class OrdersController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Staff/Orders/Index.cshtml");
    }
}
