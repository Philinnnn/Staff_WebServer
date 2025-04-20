using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;
using Staff_WebServer.Models;

namespace Staff_WebServer.Controllers;

public class SeedUsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedUsersController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> Generate()
    {
        var employees = await _context.Employees
            .Include(e => e.Position)
            .ToListAsync();

        int createdCount = 0;

        foreach (var emp in employees)
        {
            if (_context.Users.Any(u => u.ТабельныйНомер == emp.Id))
                continue;

            var email = Transliterate(GetLastAndFirstName(emp.FullName)) + "@example.com";
            var password = "Qwerty123!";

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                ТабельныйНомер = emp.Id
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) continue;

            string role = emp.Position.Name.ToLower() switch
            {
                var s when s.Contains("директор") => "Director",
                var s when s.Contains("hr") => "HR",
                var s when s.Contains("администратор") => "Admin",
                _ => "Employee"
            };

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
            createdCount++;
        }

        return Content($"✅ Готово: создано {createdCount} пользователей.");
    }

    private static string GetLastAndFirstName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return fullName.ToLower();
        return (parts[0] + "." + parts[1]).ToLower();
    }

    private static string Transliterate(string input)
    {
        var map = new Dictionary<char, string>
        {
            {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
            {'е', "e"}, {'ё', "yo"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
            {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
            {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
            {'у', "u"}, {'ф', "f"}, {'х', "h"}, {'ц', "ts"}, {'ч', "ch"},
            {'ш', "sh"}, {'щ', "sch"}, {'ь', ""}, {'ы', "y"}, {'ъ', ""},
            {'э', "e"}, {'ю', "yu"}, {'я', "ya"}, {' ', "."}
        };

        return string.Concat(input.ToLower().Select(c =>
            map.TryGetValue(c, out string value) ? value : c.ToString()
        ));
    }
}
