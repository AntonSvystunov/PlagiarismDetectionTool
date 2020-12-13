using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlagiarismDetectionTool.LuceneTools.Services;
using PlagiarismDetectionTool.Utils.Indexer;
using PlagiarismDetectionTool.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismDetectionTool.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IIndexer indexer;
        private readonly ISimilarityDetectionAlgorithm similarityDetectionAlgorithm;

        public HomeController(ILogger<HomeController> logger, IIndexer indexer, ISimilarityDetectionAlgorithm similarityDetectionAlgorithm)
        {
            this.logger = logger;
            this.indexer = indexer;
            this.similarityDetectionAlgorithm = similarityDetectionAlgorithm;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessFile([FromForm(Name = "uploadedFile")] IFormFile formFile)
        {
            IndexedDocument document;
            using (var stream = formFile.OpenReadStream())
            {
                document = indexer.ProcessDocument(formFile.FileName, stream);
            }

            var result = similarityDetectionAlgorithm.GetScore(document, DetectionStrategy.ByPhrase);
            TempData["resultModels"] = JsonConvert.SerializeObject(ResultModel.FromDictionary(result));
            return RedirectToAction("CheckResults", "Home", routeValues: new { fileName = formFile.FileName });
        }

        public IActionResult CheckResults(string fileName)
        {
            IEnumerable<ResultModel> resultModels = JsonConvert.DeserializeObject<IEnumerable<ResultModel>>(TempData["resultModels"] as string);
            ViewBag.FileName = fileName;
            return View(resultModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
