namespace XMLGenerator.Services.Models
{
    public class DocumentSection
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string Text { get; set; } = "";

        public ICollection<DocumentSection> Children { get; set; } = new List<DocumentSection>();
    }
}
