using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Staff_WebServer.Data;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "HR,Director")]
public class ProcedureController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProcedureController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Educations = _context.Educations.ToList();
        ViewBag.Positions = _context.Positions.ToList();
        return View();
    }

    public IActionResult FreeVacancies()
    {
        var result = new List<dynamic>();
        var conn = _context.Database.GetDbConnection();
        conn.Open();

        using (var command = conn.CreateCommand())
        {
            command.CommandText = @"
            SELECT *
            FROM Штатное_расписание
            WHERE общее_количество - количество_вакансий > 0
            ORDER BY количество_вакансий DESC";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        код_подразделения = reader["код_подразделения"],
                        код_должности = reader["код_должности"],
                        общее_количество = reader["общее_количество"],
                        количество_вакансий = reader["количество_вакансий"]
                    });
                }
            }
        }

        conn.Close();

        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Positions = _context.Positions.ToList();

        return View(result);
    }

    [Authorize(Roles = "HR, Director")]
    [HttpGet]
    public IActionResult EmployeeByDepartmentEducation(int? departmentId, int? educationId)
    {
        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Educations = _context.Educations.ToList();

        if (departmentId == null || educationId == null)
        {
            return View(new List<dynamic>());
        }

        var result = new List<dynamic>();
        var conn = _context.Database.GetDbConnection();
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "EXEC ВыборРаботников @depId, @eduId";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@depId";
            p1.Value = departmentId.Value;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@eduId";
            p2.Value = educationId.Value;
            cmd.Parameters.Add(p2);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        табельный_номер = reader["табельный_номер"],
                        ФИО = reader["ФИО"],
                        пол = reader["пол"],
                        дата_рождения = reader["дата_рождения"],
                        дата_приема = reader["дата_приема"],
                        оклад = reader["оклад"]
                    });
                }
            }
        }

        conn.Close();
        return View(result);
    }

    [Authorize(Roles = "HR, Director")]
    public IActionResult BusyUnits(int? departmentId, int? positionId)
    {
        ViewBag.Departments = _context.Departments.OrderBy(d => d.Name).ToList();
        ViewBag.Positions = _context.Positions.OrderBy(p => p.Name).ToList();

        var result = new List<dynamic>();
        if (departmentId.HasValue && positionId.HasValue)
        {
            var connection = _context.Database.GetDbConnection();
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "ВывестиКоличествоЗанятыхРабочихМест";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                var depParam = command.CreateParameter();
                depParam.ParameterName = "@код_подразделения";
                depParam.Value = departmentId;
                command.Parameters.Add(depParam);

                var posParam = command.CreateParameter();
                posParam.ParameterName = "@код_должности";
                posParam.Value = positionId;
                command.Parameters.Add(posParam);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            код_подразделения = reader["код_подразделения"],
                            код_должности = reader["код_должности"],
                            КоличествоЗанятыхМест = reader["КоличествоЗанятыхМест"]
                        });
                    }
                }
            }

            connection.Close();
        }

        return View(result);
    }
}
