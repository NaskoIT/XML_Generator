using System.ComponentModel.DataAnnotations;

namespace XMLGenerator.Models
{
    public class GeneratedXmlViewModel
    {
        public GeneratedXmlViewModel()
        {
        }

        public GeneratedXmlViewModel(string xml)
        {
            Xml = xml;
        }

        [Required]
        [MaxLength(10000)]
        public string? Xml { get; set; }
    }
}
