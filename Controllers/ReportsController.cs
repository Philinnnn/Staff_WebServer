using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;

[Authorize(Roles = "Director")]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult DependentAllowances(int departmentId = 1)
    {
        var result = _context.Employees
            .FromSqlRaw("SELECT * FROM fn_Рассчитать_начисления_на_иждивенцев({0})", departmentId)
            .ToList();

        return View(result);
    }
}