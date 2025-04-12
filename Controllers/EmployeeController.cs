using Microsoft.AspNetCore.Mvc;

namespace Staff_WebServer.Controllers;

public class EmployeeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}