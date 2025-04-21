using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("национальности")]
public class Nationality
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("код_национальности")]
    public int Id { get; set; }

    [Column("наименование")]
    public string Name { get; set; }

    public List<Employee> Employees { get; set; }
}
