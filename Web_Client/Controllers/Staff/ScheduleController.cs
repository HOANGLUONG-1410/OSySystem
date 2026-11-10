using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Staff
{
    [Route("Staff/Schedule")]

    public class ScheduleController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Staff/Schedule/Index.cshtml");
    }
}
