
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.KitchenManager
{
    [Route("KitchenManager/Inventory")]
    
    public class InventoryController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/KitchenManager/Inventory/Index.cshtml");
    }
}
