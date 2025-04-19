using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_WebServer.Models;

public class ApplicationUser : IdentityUser
{
    public int ТабельныйНомер { get; set; }

    public Employee Employee { get; set; }
}