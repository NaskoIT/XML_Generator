using System.ComponentModel.DataAnnotations;

namespace XMLGenerator.Models
{
    public class GenerateXmlInputModel
    {
        [Required]
        public IFormFile? DtdFile { get; set; }

        [Required]
        [MaxLength(350)]
        public string? WikipediaLink { get; set; }
    }
}
