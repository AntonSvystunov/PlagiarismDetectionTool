using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using PlagiarismDetectionTool.LuceneTools;
using PlagiarismDetectionTool.LuceneTools.Provisioning;
using PlagiarismDetectionTool.LuceneTools.Services;
using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismDetectionTool.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPlagiarismDetection(this IServiceCollection services)
        {
            services.AddSingleton<LuceneConfig>();
            services.AddSingleton<ScamConfig>();

            services.AddSingleton<Analyzer, StandardAnalyzer>(services => new StandardAnalyzer(services.GetRequiredService<LuceneConfig>().LuceneVersion));
            services.AddSingleton<Directory, RAMDirectory>(services => new RAMDirectory());
            services.AddSingleton<IIndexer, TextFileIndexer>();
            services.AddSingleton<ILuceneService, LuceneService>();
            services.AddTransient<IProvisionService, ProvisionService>();
            services.AddSingleton<IVectorTokenizer, VectorTokenizer>();

            services.AddSingleton<ISimilarityDetectionAlgorithm,ScamAlgorithm>();
        }
    }
}
