using Microsoft.AspNetCore.Mvc;

namespace Evi_Mon_CP_V01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
