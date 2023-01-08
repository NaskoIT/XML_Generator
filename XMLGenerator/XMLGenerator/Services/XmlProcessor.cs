namespace XMLGenerator.Services
{
    public class XmlProcessor : IXmlProcessor
    {
        public string GenerateXml(IFormFile file, string wikipediaLink)
        {
            return @" <?xml version=""1.0"" encoding=""windows-1251"" ?>
<!DOCTYPE recipe [
<!ELEMENT recipe (category,author,title,body)>
<!ELEMENT category (#PCDATA)>
<!ELEMENT author (#PCDATA)>
<!ELEMENT title (#PCDATA)>
<!ELEMENT body (#PCDATA)>
]>
<recipe>
<category>Cakes</category>
<author>Ralica</author>
<title>Chocolate cake</title>
<body>Products: ….
Way of preparation:….
Result: Very delicious!
</body>
</recipe>";
        }
    }
}
