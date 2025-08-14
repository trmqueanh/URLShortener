using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener.MVC.Models
{
    [Table("CustomizedURL")]
    public class HomeModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MinLength(4, ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Original URL is required")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? OriginalUrl { get; set; }

        [Required(ErrorMessage = "Number of characters is required")]
        [Range(3, 20, ErrorMessage = "Number of characters must be between 3 and 20")]
        [Display(Name = "Number of Characters")]
        public int CharacterCount { get; set; }

        public string? ShortenedUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
