using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Staff_WebServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "HR")]
public class EmployeeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public EmployeeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        // TODO: Заменить на запрос к БД для получения списка сотрудников
        return View(new List<Employee>());
    }

    public IActionResult Details(int id)
    {
        // TODO: Заменить на запрос к БД для получения данных сотрудника
        return View(new Employee { Id = id, FullName = "Тестовый Работник" });
    }
}