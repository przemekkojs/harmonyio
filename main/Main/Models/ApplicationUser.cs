using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Main.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(32), Display(Name = "First Name")]
    public required string FirstName { get; set; }
    
    [MaxLength(32), Display(Name = "Last Name")]
    public required string LastName { get; set; }
}