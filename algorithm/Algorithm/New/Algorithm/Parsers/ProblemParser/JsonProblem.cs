using Algorithm.New.Music;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Parsers.ProblemParser
{
    public class JsonProblem
    {
        [JsonPropertyName("MetreValue")]
        public int MetreValue { get; set; }

        [JsonPropertyName("MetreCount")]
        public int MetreCount { get; set; }
        public int SharpsCount { get; set; }
        public int FlatsCount { get; set; }
        public string Question { get; set; }
        public int Minor { get; set; }
        public int MaxPoints { get; set; }

        [JsonPropertyName("Task")]
        public List<ParsedFunction> Functions { get; set; }

        [JsonConstructor]
        public JsonProblem(string question, string metreCount, string metreValue, int sharpsCount, int flatsCount, int minor, string maxPoints, List<ParsedFunction> task)
        {
            MetreCount = Convert.ToInt32(metreCount);
            MetreValue = Convert.ToInt32(metreValue);
            SharpsCount = sharpsCount;
            FlatsCount = flatsCount;
            Functions = task;
            Minor = minor;
            Question = question;
            MaxPoints = Convert.ToInt32(maxPoints);
        }
    }
}
