using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Constants
{
    public static class CustomeFieldTypes
    {
        public static readonly FieldType StoreTermVectorsField = new FieldType()
        {
            IsStored = true,
            IsIndexed = true,
            IsTokenized = true,
            StoreTermVectors = true
        };
    }
}
