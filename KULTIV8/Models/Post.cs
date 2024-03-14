
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace KULTIV8.Models;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string PhotoUrl { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime DatePublished { get; set; }

    [Required]
    public DateTime LastUpdate { get; set; }

    [Required]
    public string Status { get; set; }

    [NotMapped] //Telling our DB context not to include this property in the migration, we 
                //dont want to save the image bytes in db... Acts as a placeholder for the image
    [Display(Name = "Cover Photo")]
    public IFormFile CoverPhoto { get; set; }

    [Required]
    [ForeignKey("User")]
    public string AuthorId { get; set; }

    public UserModel User { get; set; }

}

