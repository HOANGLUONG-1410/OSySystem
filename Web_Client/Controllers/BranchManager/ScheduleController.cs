using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.BranchManager
{
    [Route("BranchManager/Schedule")]

    public class ScheduleController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/BranchManager/Schedule/Index.cshtml");
    }
}
