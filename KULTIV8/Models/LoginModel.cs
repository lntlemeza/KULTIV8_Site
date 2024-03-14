using System.ComponentModel.DataAnnotations;

namespace KULTIV8.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
