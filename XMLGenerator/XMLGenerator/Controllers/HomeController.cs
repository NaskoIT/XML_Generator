using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using XMLGenerator.Models;
using XMLGenerator.Services;

namespace XMLGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IXmlProcessor xmlProcessor;
        private readonly IWebDocumentParser webDocumentParser;
        private readonly IFileService fileService;

        public HomeController(
            ILogger<HomeController> logger, 
            IXmlProcessor xmlProcessor,
            IWebDocumentParser webDocumentParser,
            IFileService fileService)
        {
            _logger = logger;
            this.xmlProcessor = xmlProcessor;
            this.webDocumentParser = webDocumentParser;
            this.fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(GenerateXmlInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.WikipediaLink == null || !model.WikipediaLink.StartsWith("https://en.wikipedia"))
            {
                ModelState.AddModelError(nameof(GenerateXmlInputModel.WikipediaLink), "Please, type a valid url address to wikipedia page!");
                return View(model);
            }
            if (model.DtdFile == null)
            {
                ModelState.AddModelError(nameof(GenerateXmlInputModel.DtdFile), "Please upload valid dtd file!");
                return View(model);
            }
            if (model.DtdFile != null && !model.DtdFile.FileName.EndsWith(".dtd"))
            {
                ModelState.AddModelError(nameof(GenerateXmlInputModel.DtdFile), "Please upload valid dtd file, the file extension should be .dtd!");
                return View(model);
            }

            var document = await this.webDocumentParser.Parse(model.WikipediaLink!);

            using var stream = new MemoryStream();
            model.DtdFile!.CopyTo(stream);
            var dtdText = Encoding.UTF8.GetString(stream.ToArray());
            string dtdFileName = fileService.Save(dtdText, "dtd");

            using var xml = this.xmlProcessor.GenerateXml(document, dtdFileName);
            var text = Encoding.UTF8.GetString(xml!.ToArray());

            return RedirectToAction(nameof(Edit), new GeneratedXmlViewModel(text));
        }

        public IActionResult Edit(GeneratedXmlViewModel model)
        {
            return View(model);
        }

        [HttpPost()]
        public IActionResult EditHandler(GeneratedXmlViewModel model)
        {
            // ModelState.AddModelError(string.Empty, "The xml content is not compatible with the DTD schema!");
            if (!ModelState.IsValid)
            {
                return View(nameof(Edit), model);
            }

            string xmlFileName = this.fileService.Save(model.Xml!, ".xml");
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(model.Xml!));
            var errors = this.xmlProcessor.ValidateXml(stream);
            if (errors.Length > 0)
            {
                const string error = "The XML file is not compatible with the DTD schema.";
                ModelState.AddModelError(nameof(GeneratedXmlViewModel.Xml), error + " " + errors.ToString());
                return View(nameof(Edit), model);
            }
            else
            {
                return File(Encoding.ASCII.GetBytes(model.Xml!), "application/octet-stream", "wikipedia_page.xml");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}