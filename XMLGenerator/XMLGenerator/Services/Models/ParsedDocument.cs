namespace XMLGenerator.Services.Models
{
    public class ParsedDocument
    {
        public string? Title { get; set; }

        public ICollection<DocumentSection> Sections { get; set; } = new List<DocumentSection>();
    }
}
