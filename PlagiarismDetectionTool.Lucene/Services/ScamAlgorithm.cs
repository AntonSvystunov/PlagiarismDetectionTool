using PlagiarismDetectionTool.Utils.Indexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlagiarismDetectionTool.LuceneTools.Services
{
    public class ScamAlgorithm : ISimilarityDetectionAlgorithm
    {
        private readonly IVectorTokenizer vectorTokenizer;
        private readonly ILuceneService luceneService;
        private readonly ScamConfig scamConfig;

        public ScamAlgorithm(IVectorTokenizer vectorTokenizer, ILuceneService luceneService, ScamConfig scamConfig)
        {
            this.vectorTokenizer = vectorTokenizer;
            this.luceneService = luceneService;
            this.scamConfig = scamConfig;
        }

        public IDictionary<string, double> GetScore(IndexedDocument document, DetectionStrategy detectionStrategy = DetectionStrategy.ByDocument)
        {
            switch(detectionStrategy)
            {
                case DetectionStrategy.ByDocument:
                    return GetScoreByDocument(document);
                case DetectionStrategy.ByPhrase:
                    return GetScoreByPhrase(document);
            }

            throw new NotImplementedException();
        }

        private IDictionary<string, double> GetScoreByPhrase(IndexedDocument indexedDocument)
        {
            var documentScores1 = new Dictionary<string, double>();
            var documentScores2 = new Dictionary<string, double>();

            var testDocumentTfVector = new Dictionary<string, long>();
            var docVectors = new Dictionary<string, Dictionary<string, long>>();
            foreach (var phrase in indexedDocument.Phrases.Where(ph => ph.Length >= 5))
            {
                var frequencyVector = vectorTokenizer.GetTokenFrequencyVector(phrase);

                long denom1 = frequencyVector.Select(kv => kv.Value * kv.Value).Sum();

                foreach (var term in frequencyVector.Keys)
                {
                    var occurences = luceneService.GetOccurences(term);
                    foreach (var document in occurences.GetDocuments())
                    {
                        var documentTerms = occurences.GetTermsVectors(document);
                        long denom2 = documentTerms.Select(kv => kv.Value * kv.Value).Sum();
                        foreach (var documentTerm in documentTerms)
                        {
                            if (frequencyVector.ContainsKey(documentTerm.Key))
                            {
                                long f1 = frequencyVector[documentTerm.Key];
                                long f2 = documentTerm.Value;

                                if ((scamConfig.Epsilon - (((double)f1 / f2) + ((double)f2 / f1))) > 0)
                                {
                                    double delta = (double)(f1 * f2) / denom1;

                                    if (!documentScores1.ContainsKey(document))
                                    {
                                        documentScores1.Add(document, delta);
                                    }
                                    else
                                    {
                                        documentScores1[document] += delta;
                                    }

                                    delta = (double)(f1 * f2) / denom2;

                                    if (!documentScores2.ContainsKey(document))
                                    {
                                        documentScores2.Add(document, delta);
                                    }
                                    else
                                    {
                                        documentScores2[document] += delta;
                                    }
                                }

                            }
                        }
                    }
                }
            }

            return NormalizeScores(documentScores1, documentScores2);
        }

        private IDictionary<string, double> GetScoreByDocument(IndexedDocument indexedDocument)
        {
            var documentScores1 = new Dictionary<string, double>();
            var documentScores2 = new Dictionary<string, double>();

            var testDocumentTfVector = new Dictionary<string, long>();
            var docVectors = new Dictionary<string, Dictionary<string, long>>();

            var phrases = indexedDocument.Phrases.Where(ph => ph.Length >= 5);
            var testFeqVector = vectorTokenizer.GetTokenFrequencyVector(string.Join(' ', phrases));

            var docToDocIds = new Dictionary<string, ISet<int>>();
            foreach (var term in testFeqVector.Keys)
            {
                var occurences = luceneService.GetDocumentsWithMatches(term);
                foreach (var kv in occurences)
                {
                    if (docToDocIds.ContainsKey(kv.Key))
                    {
                        docToDocIds[kv.Key].UnionWith(kv.Value);
                    }
                    else
                    {
                        docToDocIds.Add(kv.Key, kv.Value);
                    }
                }
            }

            var docTermVectors = luceneService.GetTermVectors(docToDocIds);
            var denom1 = testFeqVector.Select(kv => kv.Value * kv.Value).Sum();

            foreach (var documentName in docTermVectors.Keys)
            {
                var documentTerms = docTermVectors[documentName];
                var denom2 = documentTerms.Select(kv => kv.Value * kv.Value).Sum();

                foreach (var documentTerm in documentTerms)
                {
                    if (testFeqVector.ContainsKey(documentTerm.Key))
                    {
                        long f1 = testFeqVector[documentTerm.Key];
                        long f2 = documentTerm.Value;

                        if ((scamConfig.Epsilon - (((double)f1 / f2) + ((double)f2 / f1))) > 0)
                        {
                            double delta = (double)(f1 * f2) / denom1;

                            if (!documentScores1.ContainsKey(documentName))
                            {
                                documentScores1.Add(documentName, delta);
                            }
                            else
                            {
                                documentScores1[documentName] += delta;
                            }

                            delta = (double)(f1 * f2) / denom2;

                            if (!documentScores2.ContainsKey(documentName))
                            {
                                documentScores2.Add(documentName, delta);
                            }
                            else
                            {
                                documentScores2[documentName] += delta;
                            }
                        }

                    }
                }
            }

            return NormalizeScores(documentScores1, documentScores2);
        }

        private IDictionary<string, double> NormalizeScores(IDictionary<string, double> documentScores1, IDictionary<string, double> documentScores2)
        {
            var resultScore = new Dictionary<string, double>();
            var documentsMatch = documentScores1.Keys;
            foreach (var document in documentsMatch)
            {
                var score1 = documentScores1[document];
                var score2 = documentScores2[document];
                resultScore.Add(document, Math.Max(score1, score2));
            }

            return resultScore;
        }
    }
}
