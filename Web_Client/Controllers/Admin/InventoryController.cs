
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Admin
{
    [Route("Admin/Inventory")]
    
    public class InventoryController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Admin/Inventory/Index.cshtml");
    }
}
