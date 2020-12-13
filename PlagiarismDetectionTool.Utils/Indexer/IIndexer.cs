using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlagiarismDetectionTool.Utils.Indexer
{
    public interface IIndexer
    {
        IndexedDocument ProcessDocument(string filePath);
        IndexedDocument ProcessDocument(string filename, Stream stream);
    }
}
