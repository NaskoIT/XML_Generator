using System.Text;
using XMLGenerator.Services.Models;

namespace XMLGenerator.Services
{
    public interface IXmlProcessor
    {
        MemoryStream? GenerateXml(ParsedDocument? document, string dtdFileName);

        StringBuilder ValidateXml(MemoryStream stream);
    }
}
