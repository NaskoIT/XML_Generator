using AngleSharp.Dom;
using AngleSharp;
using XMLGenerator.Services.Models;
using AngleSharp.Html.Dom;

namespace XMLGenerator.Services
{
    public class WebDocumentParser : IWebDocumentParser
    {
        public async Task<ParsedDocument> Parse(string url)
        {
            AngleSharp.IConfiguration config = Configuration.Default.WithDefaultLoader();
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(url);

            var blacklistTags = new HashSet<string>() { "References", "External links" };
            var content = ParseDocumentContent(document)
              .Where(x => !string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Id) && !blacklistTags.Contains(x.Title))
              .ToList();

            LoadContent(document, content);


            return new ParsedDocument()
            {
                Title = document.QuerySelector(".mw-page-title-main")?.TextContent,
                Sections = content
            };
        }

        static void LoadContent(IDocument document, ICollection<DocumentSection>? sections, int index = 2)
        {
            if (sections == null || !sections.Any())
            {
                return;
            }

            foreach (var s in sections)
            {
                var rawSection = document.GetElementById(s.Id!);
                var element = rawSection?.Parent?.NextSibling;
                var blockers = new HashSet<string>() { $"H{index - 1}", $"H{index}", $"H{index + 1}" };
                while (element != null && !blockers.Contains(element.NodeName))
                {
                    s.Text += element.TextContent;
                    element = element.NextSibling;
                }

                LoadContent(document, s.Children, index + 1);
            }
        }

        static ICollection<DocumentSection> ParseDocumentContent(IDocument document)
        {
            var sections = new List<DocumentSection>();
            ParseContent(sections, document.GetElementsByTagName("body")?.FirstOrDefault(), 1);
            return sections;
        }

        static void PrintDocument(ICollection<DocumentSection> sections, int indent = 0)
        {
            if (sections == null || !sections.Any())
            {
                return;
            }

            foreach (var s in sections)
            {
                Console.WriteLine($"{new string('-', indent * 4)}{s.Title} {s.Text}");
                PrintDocument(s.Children, indent + 1);
            }
        }

        static void ParseContent(ICollection<DocumentSection> sections, IElement? element, int index)
        {
            if (sections == null || element == null)
            {
                return;
            }

            var rawSections = element.GetElementsByClassName($"toclevel-{index}");
            if (rawSections == null || rawSections.Count() == 0)
            {
                return;
            }

            foreach (var rawSection in rawSections)
            {
                var section = new DocumentSection()
                {
                    Id = (rawSection.GetElementsByTagName("a")?.FirstOrDefault() as IHtmlAnchorElement)?.Href?.Split('#').LastOrDefault(),
                    Title = rawSection.QuerySelector(".toctext")?.TextContent
                };

                sections.Add(section);

                ParseContent(section.Children, rawSection, index + 1);
            }
        }
    }
}
