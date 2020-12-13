using Lucene.Net.Index;
using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public interface ILuceneService
    {
        public void AddDocuments(IEnumerable<IndexedDocument> indexedDocument);
        public IndexWriter GetIndexWriter();
        public DocumentOccurence GetOccurences(string term);
        public Dictionary<string, IDictionary<string, long>> GetTermVectors(IDictionary<string, ISet<int>> nameToDocId);
        public IDictionary<string, ISet<int>> GetDocumentsWithMatches(string term);
    }
}
