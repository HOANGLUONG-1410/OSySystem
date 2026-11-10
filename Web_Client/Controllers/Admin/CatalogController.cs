
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Admin
{
    [Route("Admin/Catalog")]
    
    public class CatalogController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Admin/Catalog/Index.cshtml");
    }
}
