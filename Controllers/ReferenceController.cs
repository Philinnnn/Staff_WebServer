using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

[Authorize]
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
        ViewBag.Categories = _context.PositionCategories.ToList();
        return View();
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    public IActionResult Add(string type, string value, int categoryId = 0)
    {
        if (string.IsNullOrWhiteSpace(value)) return RedirectToAction("All");

        switch (type)
        {
            case "Department":
                var newDeptId = (_context.Departments.Max(d => (int?)d.Id) ?? 0) + 1;
                _context.Departments.Add(new Department { Id = newDeptId, Name = value });
                break;
            case "Nationality":
                var newNatId = (_context.Nationalities.Max(n => (int?)n.Id) ?? 0) + 1;
                _context.Nationalities.Add(new Nationality { Id = newNatId, Name = value });
                break;
            case "Education":
                var newEduId = (_context.Educations.Max(e => (int?)e.Id) ?? 0) + 1;
                _context.Educations.Add(new Education { Id = newEduId, Name = value });
                break;
            case "Position":
                var newPosId = (_context.Positions.Max(p => (int?)p.Id) ?? 0) + 1;
                _context.Positions.Add(new Position { Id = newPosId, Name = value, CategoryId = categoryId });
                break;
            case "PensionFund":
                var newPfId = (_context.PensionFunds.Max(p => (int?)p.Id) ?? 0) + 1;
                _context.PensionFunds.Add(new PensionFund { Id = newPfId, Name = value });
                break;
            case "OrderType":
                var newOtId = (_context.OrderTypes.Max(t => (int?)t.Id) ?? 0) + 1;
                _context.OrderTypes.Add(new OrderType { Id = newOtId, Name = value });
                break;
        }

        _context.SaveChanges();
        return RedirectToAction("All");
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    public IActionResult Update(string type, int id, string newValue, int? categoryId)
    {
        if (string.IsNullOrWhiteSpace(newValue)) return RedirectToAction("All");

        switch (type)
        {
            case "Department":
                var dept = _context.Departments.Find(id);
                if (dept != null) dept.Name = newValue;
                break;
            case "Nationality":
                var nat = _context.Nationalities.Find(id);
                if (nat != null) nat.Name = newValue;
                break;
            case "Education":
                var edu = _context.Educations.Find(id);
                if (edu != null) edu.Name = newValue;
                break;
            case "Position":
                var pos = _context.Positions.Find(id);
                if (pos != null)
                {
                    pos.Name = newValue;
                    if (categoryId != null) pos.CategoryId = categoryId.Value;
                }
                break;
            case "PensionFund":
                var pf = _context.PensionFunds.Find(id);
                if (pf != null) pf.Name = newValue;
                break;
            case "OrderType":
                var ot = _context.OrderTypes.Find(id);
                if (ot != null) ot.Name = newValue;
                break;
        }

        _context.SaveChanges();
        return RedirectToAction("All");
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    public IActionResult Delete(string type, int id)
    {
        try
        {
            switch (type)
            {
                case "Department":
                    _context.Departments.Remove(_context.Departments.Find(id));
                    break;
                case "Nationality":
                    _context.Nationalities.Remove(_context.Nationalities.Find(id));
                    break;
                case "Education":
                    _context.Educations.Remove(_context.Educations.Find(id));
                    break;
                case "Position":
                    _context.Positions.Remove(_context.Positions.Find(id));
                    break;
                case "PensionFund":
                    _context.PensionFunds.Remove(_context.PensionFunds.Find(id));
                    break;
                case "OrderType":
                    _context.OrderTypes.Remove(_context.OrderTypes.Find(id));
                    break;
            }
            _context.SaveChanges();
        }
        catch
        {
            TempData["Error"] = "Невозможно удалить элемент. Возможно, он используется.";
        }

        return RedirectToAction("All");
    }
}
