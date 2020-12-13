using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.Utils.Indexer
{
    public class IndexedDocument
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public IEnumerable<string> Phrases { get; set; }
    }
}
