using Lucene.Net.Documents;
using PlagiarismDetectionTool.LuceneTools.Constants;
using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Extensions
{
    public static class IndexedDocumentExtensions
    {
        public static IEnumerable<Document> ToLuceneDocuments(this IndexedDocument indexedDocument)
        {
            var result = new List<Document>();

            foreach (var phrase in indexedDocument.Phrases)
            {
                var document = new Document();
                document.Add(new Field(IndexFieldNames.Text, phrase, CustomeFieldTypes.StoreTermVectorsField));
                document.Add(new StringField(IndexFieldNames.File, indexedDocument.FileName, Field.Store.YES));
                document.Add(new StringField(IndexFieldNames.Id, indexedDocument.Id.ToString(), Field.Store.YES));
                result.Add(document);
            }

            return result;
        }
    }
}
