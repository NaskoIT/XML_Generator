using System.Text;
using XMLGenerator.Services.Models;

namespace XMLGenerator.Services
{
    public interface IXmlProcessor
    {
        MemoryStream? GenerateXml(ParsedDocument? document);

        StringBuilder ValidateXml(MemoryStream stream);
    }
}
