using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_WebServer.Data;

namespace Staff_WebServer.Controllers;

[Authorize(Roles = "Director")]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Reports()
    {
        ViewBag.Departments = _context.Departments.OrderBy(d => d.Name).ToList();
        return View();
    }

    public IActionResult PensionerReport()
    {
        var result = new List<dynamic>();

        var connection = _context.Database.GetDbConnection();
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            SELECT 
                табельный_номер,
                ФИО,
                пол,
                дата_рождения,
                дата_увольнения,
                dbo.fn_ПолныхЛет(дата_рождения) AS возраст
            FROM Работники
            WHERE дата_увольнения IS NOT NULL 
              AND YEAR(дата_увольнения) = YEAR(GETDATE())
              AND (
                  (пол = N'м' AND dbo.fn_ПолныхЛет(дата_рождения) >= 63)
                  OR (пол = N'ж' AND dbo.fn_ПолныхЛет(дата_рождения) >= 58)
              )";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        FullName = reader["ФИО"].ToString(),
                        Gender = reader["пол"].ToString(),
                        BirthDate = Convert.ToDateTime(reader["дата_рождения"]),
                        DismissDate = Convert.ToDateTime(reader["дата_увольнения"]),
                        Age = reader["возраст"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["возраст"])
                    });
                }
            }
        }

        connection.Close();
        return View(result);
    }

    public IActionResult DependentAllowances(int departmentId)
    {
        var result = new List<dynamic>();

        var connection = _context.Database.GetDbConnection();
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = """

                                              SELECT 
                                                  табельный_номер,
                                                  ФИО,
                                                  оклад,
                                                  кол_во_иждивенцев,
                                                  dbo.fn_Рассчитать_начисления_на_иждивенцев(табельный_номер) AS Начисления
                                              FROM Работники
                                              WHERE код_подразделения = @dep
                                  """;

            var param = command.CreateParameter();
            param.ParameterName = "@dep";
            param.Value = departmentId;
            command.Parameters.Add(param);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        Id = reader["табельный_номер"],
                        FullName = reader["ФИО"].ToString(),
                        Salary = Convert.ToDecimal(reader["оклад"]),
                        Dependents = Convert.ToInt32(reader["кол_во_иждивенцев"]),
                        Total = Convert.ToInt32(reader["Начисления"])
                    });
                }
            }
        }

        connection.Close();
        return View(result);
    }
}