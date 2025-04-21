using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("должности")]
public class Position
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("код_должности")]
    public int Id { get; set; }

    [Column("наименование")]
    public string Name { get; set; }

    [Column("код_категории")]
    public int CategoryId { get; set; }

    public PositionCategory Category { get; set; }

    public List<Employee> Employees { get; set; }
}
