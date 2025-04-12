using Microsoft.AspNetCore.Mvc;

namespace Staff_WebServer.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Worker()
        {
            return View();
        }
    }
}