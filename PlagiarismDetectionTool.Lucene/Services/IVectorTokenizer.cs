using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public interface IVectorTokenizer
    {
        public IDictionary<string, long> GetTokenFrequencyVector(string phrase);
        /*public IDictionary<string, long> GetTokenFrequencyVector(string[] phrases);*/
    }
}
