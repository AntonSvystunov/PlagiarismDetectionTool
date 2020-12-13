using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Linq;
using System.Text;

namespace PlagiarismDetectionTool.Utils
{
    public static class WordToTextConversion
    {
        public static string ToPlainText(System.IO.Stream stream)
        {
            using WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(stream, false);
            var hideSpellingErrors = new HideSpellingErrors();
            hideSpellingErrors.Val = OnOffValue.FromBoolean(true);
            wordprocessingDocument.MainDocumentPart.Document.Append(hideSpellingErrors);

            var sb = new StringBuilder();
            foreach (var paragraph in wordprocessingDocument.MainDocumentPart.RootElement.Descendants<Paragraph>())
            {
                foreach (var run in paragraph.Elements<Run>())
                {
                    string text = run.Elements<Text>().Aggregate("", (s, t) => s + " " + t.Text);
                    run.RemoveAllChildren<Text>();
                    run.AppendChild(new Text(text));
                    sb.Append(run.InnerText);
                }
            }
            return sb.ToString();
        }
    }
}
