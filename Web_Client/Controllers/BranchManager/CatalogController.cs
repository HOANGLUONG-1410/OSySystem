
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.BranchManager
{
    [Route("BranchManager/Catalog")]
    
    public class CatalogController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/BranchManager/Catalog/Index.cshtml");
    }
}
