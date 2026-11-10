
using Microsoft.AspNetCore.Mvc;

namespace Web_Client.Controllers.KitchenManager
{
    [Route("KitchenManager/Transfers")]
    
    public class TransfersController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index() => View("~/Views/KitchenManager/Transfers/Index.cshtml");
    }
}
