
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Staff
{
    [Route("Staff/Inventory")]
    
    public class InventoryController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Staff/Inventory/Index.cshtml");
    }
}
