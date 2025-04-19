using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

[Table("приказы")]
public class Order
{
    [Key]
    [Column("номер_приказа")]
    public int Id { get; set; }

    [Column("дата")]
    public DateTime Date { get; set; }

    [Column("табельный_номер")]
    public int EmployeeId { get; set; }

    [Column("код_типа_приказа")]
    public int OrderTypeId { get; set; }

    [Column("текст_приказа")]
    public string Text { get; set; }

    public Employee Employee { get; set; }
    public OrderType OrderType { get; set; }
}
