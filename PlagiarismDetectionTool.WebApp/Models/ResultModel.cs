using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismDetectionTool.WebApp.Models
{
    [Serializable]
    public class ResultModel
    {
        public string FileName { get; set; }
        public double Score { get; set; }

        public static IEnumerable<ResultModel> FromDictionary(IDictionary<string, double> dict)
        {
            return dict.Select(kv => new ResultModel { FileName = kv.Key, Score = kv.Value }).ToArray();
        }
    }
}
