using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.Admin
{
    [Route("Admin/ScheduleLogs")]

    public class ScheduleLogsController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/Admin/ScheduleLogs/Index.cshtml");
    }
}
