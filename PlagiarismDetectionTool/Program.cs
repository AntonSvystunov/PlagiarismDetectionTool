using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PlagiarismDetectionTool.LuceneTools.Extensions;
using PlagiarismDetectionTool.LuceneTools.Provisioning;
using PlagiarismDetectionTool.LuceneTools.Services;
using PlagiarismDetectionTool.Utils;
using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace PlagiarismDetectionTool
{
    class Program
    {
        static Lucene.Net.Util.LuceneVersion luceneVersion = Lucene.Net.Util.LuceneVersion.LUCENE_48;
        static double EPS = 2.5;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var directoryInfo = new DirectoryInfo(@"C:\Users\anton\Desktop\Train");

            Analyzer analyzer = new StandardAnalyzer(luceneVersion);

            Lucene.Net.Store.Directory directory = new RAMDirectory();
            var luceneService = new LuceneService(directory, analyzer, new LuceneTools.LuceneConfig { LuceneVersion = luceneVersion });
            var indexer = new TextFileIndexer();
            var provisioningService = new ProvisionService(luceneService, indexer);
            var vectorTokenizer = new VectorTokenizer(analyzer);

            provisioningService.ProvisionFromDirectory(directoryInfo);
            DirectoryInfo testDirectory = new DirectoryInfo(@"C:\Users\anton\Desktop\Test");

            var testDoc = testDirectory.GetFiles().Select(t => indexer.ProcessDocument(t.FullName)).FirstOrDefault();

            var scam = new ScamAlgorithm(vectorTokenizer, luceneService, new ScamConfig());
            
            foreach (var doc in scam.GetScore(testDoc, DetectionStrategy.ByDocument))
            {
                if (doc.Value > 0.1)
                    Console.WriteLine($"Doc: {doc.Key}, Score = {doc.Value}");
            }

            Console.WriteLine();
            foreach (var doc in scam.GetScore(testDoc, DetectionStrategy.ByPhrase))
            {
                if (doc.Value > 0.1)
                    Console.WriteLine($"Doc: {doc.Key}, Score = {doc.Value}");
            }

        }
    }
}
