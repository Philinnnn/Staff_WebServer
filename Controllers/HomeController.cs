using Microsoft.AspNetCore.Mvc;

namespace Staff_WebServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}