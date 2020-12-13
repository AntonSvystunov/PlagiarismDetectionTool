using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public interface ISimilarityDetectionAlgorithm
    {
        public IDictionary<string, double> GetScore(IndexedDocument document, DetectionStrategy detectionStrategy = DetectionStrategy.ByDocument);
    }
}
