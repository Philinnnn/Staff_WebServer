using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("пенсионные_фонды")]
public class PensionFund
{
    [Key]
    [Column("код_фонда")]
    public int Id { get; set; }

    [Column("наименование")]
    public string Name { get; set; }

    public List<Employee> Employees { get; set; }
}
