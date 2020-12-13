using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlagiarismDetectionTool.Utils.Indexer
{
    public class TextFileIndexer : IIndexer
    {
        private static string supportedExtensions = ".txt|.doc|.docx";
        public IndexedDocument ProcessDocument(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open);
            return ProcessDocument(filePath, fileStream);
        }

        public IndexedDocument ProcessDocument(string fileName, Stream stream)
        {
            var extension = Path.GetExtension(fileName);
            if (!supportedExtensions.Contains(extension))
            {
                throw new Exception($"Extension {extension} not supported");
            }

            var text = "";
            if (extension == "txt")
            {
                using var reader = new StreamReader(stream);
                text = reader.ReadToEnd();
            }
            else
            {
                text = WordToTextConversion.ToPlainText(stream);
            }

            return new IndexedDocument
            {
                FileName = Path.GetFileName(fileName),
                Phrases = text.Split('.', '!', '?'),
                Id = Guid.NewGuid()
            };
        }
    }
}
