using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.BranchManager
{
    [Route("BranchManager/Personnel")]

    public class PersonnelController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/BranchManager/Personnel/Index.cshtml");
    }
}
