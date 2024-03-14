using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KULTIV8.Models;

public class UserModel : IdentityUser
{
    [Required]
    public string? Profile_Pic { get; set; }

    [Required]
    public DateTime DateCreated { get; set; } = DateTime.Now;

    [Required]
    public string Role { get; set; }

    [NotMapped]
    public IFormFile ProfilePic { get; set; }

    public string? Name { get; set;}

    public string? Surname { get; set; }

}
