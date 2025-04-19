using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("типы_приказов")]
public class OrderType
{
    [Key]
    [Column("код_типа_приказа")]
    public int Id { get; set; }

    [Column("наименование")]
    public string Name { get; set; }

    public List<Order> Orders { get; set; }
}
