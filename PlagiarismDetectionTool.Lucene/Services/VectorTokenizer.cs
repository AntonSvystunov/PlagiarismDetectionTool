using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public class VectorTokenizer: IVectorTokenizer
    {
        private readonly Analyzer analyzer;

        public VectorTokenizer(Analyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public IDictionary<string, long> GetTokenFrequencyVector(string phrase)
        {
            var tokenStream = analyzer.GetTokenStream(null, new StringReader(phrase));
            tokenStream.Reset();

            var frequencyVector = new Dictionary<string, long>();

            while (tokenStream.IncrementToken())
            {
                var termAttr = tokenStream.GetAttribute<Lucene.Net.Analysis.TokenAttributes.ICharTermAttribute>();
                var analyzedTerm = termAttr.ToString();
                if (frequencyVector.ContainsKey(analyzedTerm))
                {
                    frequencyVector[analyzedTerm] += 1;
                }
                else
                {
                    frequencyVector.Add(analyzedTerm, 1);
                }

            }
            tokenStream.End();
            tokenStream.Dispose();

            return frequencyVector;
        }
    }
}
