using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

public class Order
{
    [Key]
    public int OrderNumber { get; set; }
    public DateTime Date { get; set; }

    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    [ForeignKey("OrderType")]
    public int OrderTypeCode { get; set; }
    public OrderType OrderType { get; set; }

    public string Text { get; set; }
}