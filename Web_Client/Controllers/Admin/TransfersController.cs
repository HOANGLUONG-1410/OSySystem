
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Admin
{
    [Route("Admin/Transfers")]
    
    public class TransfersController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Admin/Transfers/Index.cshtml");
    }
}
