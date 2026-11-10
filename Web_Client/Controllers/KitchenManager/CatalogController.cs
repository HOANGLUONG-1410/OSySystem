
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.KitchenManager
{
    [Route("KitchenManager/Catalog")]
    
    public class CatalogController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/KitchenManager/Catalog/Index.cshtml");
    }
}
