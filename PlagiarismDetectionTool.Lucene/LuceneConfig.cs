using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools
{
    public class LuceneConfig
    {
        public Lucene.Net.Util.LuceneVersion LuceneVersion { get; set; } = Lucene.Net.Util.LuceneVersion.LUCENE_48;
    }
}
