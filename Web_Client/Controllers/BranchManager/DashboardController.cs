
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.BranchManager
{
    [Route("BranchManager/Dashboard")]
    
    public class DashboardController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/BranchManager/Dashboard/Index.cshtml");
    }
}
