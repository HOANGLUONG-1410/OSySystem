using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.BranchManager
{
    [Route("BranchManager/Transfers")]

    public class TransfersController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/BranchManager/Transfers/Index.cshtml");
    }
}
