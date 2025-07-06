using Microsoft.AspNetCore.Mvc;

namespace MyMvcApp.Controllers
{
    public class UserController : Controller
    {
        [Route("user")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
