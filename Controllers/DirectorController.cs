using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;

namespace Staff_WebServer.Controllers;

public class DirectorController : Controller
{
    private readonly ApplicationDbContext _context;

    public DirectorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult EmployeesByDepartment()
    {
        var departments = _context.Departments
            .Include(d => d.Employees)
            .ThenInclude(e => e.Position)
            .OrderBy(d => d.Name)
            .ToList();

        return View(departments);
    }
}

