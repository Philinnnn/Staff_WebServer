using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;

[Authorize(Roles = "Employee")]
public class ReferenceController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReferenceController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult All()
    {
        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Nationalities = _context.Nationalities.ToList();
        ViewBag.Educations = _context.Educations.ToList();
        ViewBag.Positions = _context.Positions.Include(p => p.Category).ToList();
        ViewBag.PensionFunds = _context.PensionFunds.ToList();
        ViewBag.OrderTypes = _context.OrderTypes.ToList();

        return View();
    }
}