using PlagiarismDetectionTool.LuceneTools.Services;
using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Provisioning
{
    public class ProvisionService : IProvisionService
    {
        private readonly ILuceneService luceneService;
        private readonly IIndexer indexer;

        public ProvisionService(ILuceneService luceneService, IIndexer indexer)
        {
            this.luceneService = luceneService;
            this.indexer = indexer;
        }

        public void ProvisionFromDirectory(string directoryName)
        {
            var directoryInfo = new DirectoryInfo(@"C:\Users\anton\Desktop\Train");
            ProvisionFromDirectory(directoryInfo);
        }

        public void ProvisionFromDirectory(DirectoryInfo directory)
        {
            var indexedFiles = directory.GetFiles().Select(file => indexer.ProcessDocument(file.FullName));
            luceneService.AddDocuments(indexedFiles);
        }
    }
}
