using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public class DocumentOccurence
    {
        public Dictionary<string, Dictionary<string, long>> DocumentVectors { get; } = new Dictionary<string, Dictionary<string, long>>();
        public Dictionary<string, ISet<int>> DocumentIds { get; } = new Dictionary<string, ISet<int>>(); 
        public IEnumerable<string> GetDocuments() => DocumentVectors.Keys;
        public IDictionary<string, long> GetTermsVectors(string document) => DocumentVectors[document];
        public void AddOccurence(int docId, string filename, Terms terms)
        {
            if (terms is null)
            {
                throw new ArgumentNullException(nameof(terms));
            }

            var termVector = new Dictionary<string, long>();
            foreach (var term in terms)
            {
                var termString = term.Term.Utf8ToString();
                termVector.Add(termString, term.TotalTermFreq);
            }

            if (!DocumentVectors.ContainsKey(filename))
            {
                DocumentVectors.Add(filename, new Dictionary<string, long>());
            }
            else
            {
                DocumentVectors[filename] = Merge(DocumentVectors[filename], termVector);
            }

            if (!DocumentIds.ContainsKey(filename))
            {
                DocumentIds.Add(filename, new HashSet<int>() { docId });
            }
            else
            {
                DocumentIds[filename].Add(docId);
            }
        }

        private Dictionary<string, long> Merge(Dictionary<string, long> a, Dictionary<string, long> b)
        {
            var result = new Dictionary<string, long>(a);

            foreach (var kv in b)
            {
                if (result.ContainsKey(kv.Key))
                {
                    result[kv.Key] += kv.Value;
                }
                else
                {
                    result.Add(kv.Key, kv.Value);
                }
            }

            return result;
        }
    }
}
