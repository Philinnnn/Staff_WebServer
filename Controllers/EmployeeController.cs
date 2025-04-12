using Microsoft.AspNetCore.Mvc;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

public class EmployeeController : Controller
{
    public IActionResult Index()
    {
        // TODO: Временный список (замени на запрос к БД)
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FullName = "Королюк Виталий Викторович", Salary = 300000, Gender = "М" },
            new Employee { Id = 2, FullName = "Сидорова Анна Петровна", Salary = 250000, Gender = "Ж" }
        };

        return View(employees);
    }

    public IActionResult Details(int id)
    {
        var employee = new Employee { Id = id, FullName = "Тестовый Работник" };
        return View(employee);
    }
}