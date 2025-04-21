using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("подразделения")]
public class Department
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("код_подразделения")]
    public int Id { get; set; }

    [Column("наименование")]
    public string Name { get; set; }

    [Column("табельный_номер_начальника")]
    public int? HeadEmployeeId { get; set; }

    public Employee Head { get; set; }

    public List<Employee> Employees { get; set; }
}
