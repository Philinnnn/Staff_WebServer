using System.ComponentModel.DataAnnotations;

namespace Staff_WebServer.Models;

public class PensionFund
{
    [Key]
    public int PensionFundCode { get; set; }
    public string Name { get; set; }
}