using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("категории_должностей")]
public class PositionCategory
{
    [Key]
    [Column("код_категории")]
    public int Id { get; set; }

    [Column("наименование")]
    public string Name { get; set; }

    public List<Position> Positions { get; set; }
}
