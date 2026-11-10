
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.KitchenManager
{
    [Route("KitchenManager/Dashboard")]
    
    public class DashboardController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/KitchenManager/Dashboard/Index.cshtml");
    }
}
