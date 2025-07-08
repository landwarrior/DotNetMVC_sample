using Microsoft.AspNetCore.Mvc;

namespace MyMvcApp.Controllers
{
    public class PrivacyController : BaseController
    {
        [Route("privacy")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
