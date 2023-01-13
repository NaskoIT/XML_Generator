using System.Text;
using System.Xml;
using XMLGenerator.Services.Models;

namespace XMLGenerator.Services
{
    public class XmlProcessor : IXmlProcessor
    {
        public MemoryStream? GenerateXml(ParsedDocument? document)
        {
            if (document == null)
            {
                return null;
            }

            var xmlDocument = BuildXmlDocument(document.Sections, string.IsNullOrEmpty(document.Title) ? "root" : document.Title);
            using var stream = new MemoryStream();
            xmlDocument.Save(stream);
            return stream;
        }

        public StringBuilder ValidateXml(MemoryStream stream)
        {
            var messages = new StringBuilder();
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.DTD,
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = new XmlUrlResolver()
            };
            settings.ValidationEventHandler += (sender, args) => messages.AppendLine(args.Message);
            var reader = XmlReader.Create(stream, settings);

            while (reader.Read())
            {
            }

            return messages;
        }

        static XmlDocument BuildXmlDocument(ICollection<DocumentSection> sections, string title)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement!;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement mainElement = doc.CreateElement(string.Empty, ToXmlTag(title), string.Empty);
            doc.AppendChild(mainElement);

            BuildXmlDocumentRecursively(sections, doc, mainElement);

            return doc;
        }

        static string ToXmlTag(string str)
        {
            return string.Join('_', str.Split(new char[] { ' ', ',', '"', '`' }, StringSplitOptions.RemoveEmptyEntries));
        }

        static void BuildXmlDocumentRecursively(ICollection<DocumentSection> sections, XmlDocument doc, XmlElement element)
        {
            if (sections == null || !sections.Any() || element == null)
            {
                return;
            }

            foreach (var s in sections)
            {
                if (string.IsNullOrEmpty(s.Title))
                {
                    continue;
                }

                var title = ToXmlTag(s.Title);
                try
                {
                    XmlElement newElement = doc.CreateElement(string.Empty, title, string.Empty);
                    element.AppendChild(newElement);
                    if (s.Children.Any())
                    {
                        BuildXmlDocumentRecursively(s.Children, doc, newElement);
                    }
                    else if (!string.IsNullOrEmpty(s.Text))
                    {
                        XmlText text = doc.CreateTextNode(s.Text);
                        newElement.AppendChild(text);
                    }

                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}
