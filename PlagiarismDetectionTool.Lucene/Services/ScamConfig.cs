using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public class ScamConfig
    {
        public double Epsilon { get; set; } = 2.5;
        public double Threashold { get; set; } = 0.2;
        public uint MinPhrase { get; set; } = 5;
    }
}
