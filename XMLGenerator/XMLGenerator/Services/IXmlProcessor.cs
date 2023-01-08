namespace XMLGenerator.Services
{
    public interface IXmlProcessor
    {
        string GenerateXml(IFormFile file, string wikipediaLink);
    }
}
