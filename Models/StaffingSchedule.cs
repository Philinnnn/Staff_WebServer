using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("штатное_расписание")]
public class StaffSchedule
{
    [Key]
    [Column("код_подразделения", Order = 0)]
    public int DepartmentId { get; set; }

    [Key]
    [Column("код_должности", Order = 1)]
    public int PositionId { get; set; }

    [Column("общее_количество")]
    public int TotalPositions { get; set; }

    [Column("количество_вакансий")]
    public int Vacancies { get; set; }

    public Department Department { get; set; }
    public Position Position { get; set; }
}
