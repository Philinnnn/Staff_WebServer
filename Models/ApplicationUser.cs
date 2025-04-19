using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

public class ApplicationUser : IdentityUser
{
    [Column("табельный_номер")]
    public int ТабельныйНомер { get; set; }

    public Employee Employee { get; set; }
}