using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using PlagiarismDetectionTool.LuceneTools.Constants;
using PlagiarismDetectionTool.LuceneTools.Extensions;
using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public class LuceneService: ILuceneService
    {
        private readonly Lucene.Net.Store.Directory directory;
        private readonly Analyzer analyzer;
        private readonly LuceneConfig luceneConfig;

        public LuceneService(Lucene.Net.Store.Directory directory, Analyzer analyzer, LuceneConfig luceneConfig)
        {
            this.directory = directory;
            this.analyzer = analyzer;
            this.luceneConfig = luceneConfig;
        }

        public void AddDocuments(IEnumerable<IndexedDocument> indexedDocument)
        {
            var writer = GetIndexWriter();
            
            foreach (var meta in indexedDocument)
            {
                writer.AddDocuments(meta.ToLuceneDocuments());
            }

            writer.Commit();
            writer.Dispose();
        }

        public IDictionary<string, ISet<int>> GetDocumentsWithMatches(string term)
        {
            var result = new Dictionary<string, ISet<int>>();
            var indexWriter = GetIndexWriter();
            var reader = indexWriter.GetReader(true);

            TermQuery query = new TermQuery(new Term(IndexFieldNames.Text, term));
            BooleanQuery theQuery = new BooleanQuery(); theQuery.Add(query, Occur.SHOULD);
            IndexSearcher searcher = new IndexSearcher(reader);

            var searchResults = searcher.Search(query, 10);

            var matches = new HashSet<int>();
            foreach (var docId in searchResults.ScoreDocs)
            {
                Lucene.Net.Documents.Document doc = searcher.Doc(docId.Doc);
                //result.AddOccurence(docId.Doc, doc.Get(IndexFieldNames.File), reader.GetTermVector(docId.Doc, IndexFieldNames.Text));
                var documentName = doc.Get(IndexFieldNames.File);
                if (result.ContainsKey(documentName))
                {
                    result[documentName].Add(docId.Doc);
                }
                else
                {
                    result.Add(documentName, new HashSet<int>() { docId.Doc });
                }
            }

            reader.Dispose();
            indexWriter.Dispose();

            return result;
        }

        public IndexWriter GetIndexWriter()
        {
            return new IndexWriter(directory, new IndexWriterConfig(luceneConfig.LuceneVersion, analyzer));
        }

        public DocumentOccurence GetOccurences(string term)
        {
            var result = new DocumentOccurence();
            var indexWriter = GetIndexWriter();
            var reader = indexWriter.GetReader(true);

            TermQuery query = new TermQuery(new Term(IndexFieldNames.Text, term));
            BooleanQuery theQuery = new BooleanQuery(); theQuery.Add(query, Occur.SHOULD);
            IndexSearcher searcher = new IndexSearcher(reader);

            var searchResults = searcher.Search(query, 10);

            var matches = new HashSet<int>();
            foreach (var docId in searchResults.ScoreDocs)
            {
                Lucene.Net.Documents.Document doc = searcher.Doc(docId.Doc);
                result.AddOccurence(docId.Doc, doc.Get(IndexFieldNames.File), reader.GetTermVector(docId.Doc, IndexFieldNames.Text));
            }
            
            reader.Dispose();
            indexWriter.Dispose();

            return result;
        }

        public Dictionary<string, IDictionary<string, long>> GetTermVectors(IDictionary<string, ISet<int>> nameToDocId)
        {
            var result = new Dictionary<string, IDictionary<string, long>>();
            var indexWriter = GetIndexWriter();
            var reader = indexWriter.GetReader(true);

            foreach (var docNamePair in nameToDocId)
            {
                string documentName = docNamePair.Key;
                ISet<int> documentIds = docNamePair.Value;
                var termDictionary = new Dictionary<string, long>();

                foreach (var docId in documentIds)
                {
                    Terms terms = reader.GetTermVector(docId, IndexFieldNames.Text);

                    foreach (var term in terms)
                    {
                        var termLabel = term.Term.Utf8ToString();
                        if (termDictionary.ContainsKey(termLabel))
                        {
                            termDictionary[termLabel] += 1;
                        }
                        else
                        {
                            termDictionary.Add(termLabel, 1);
                        }
                    }
                }

                result.Add(documentName, termDictionary);
            }
            reader.Dispose();
            indexWriter.Dispose();

            return result;
        }
    }
}
