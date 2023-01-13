using XMLGenerator.Services.Models;

namespace XMLGenerator.Services
{
    public interface IWebDocumentParser
    {
        Task<ParsedDocument> Parse(string url);
    }
}
