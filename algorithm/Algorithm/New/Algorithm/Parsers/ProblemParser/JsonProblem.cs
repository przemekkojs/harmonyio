using Algorithm.New.Music;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Parsers.ProblemParser
{
    public class JsonProblem
    {
        public int MetreValue { get; set; }
        public int MetreCount { get; set; }
        public int SharpsCount { get; set; }
        public int FlatsCount { get; set; }
        public string Question { get; set; }
        public int Minor { get; set; }
        public List<ParsedFunction> Task { get; set; } = [];

        [JsonConstructor]
        public JsonProblem(string question, int metreCount, int metreValue, int sharpsCount, int flatsCount, int minor, List<ParsedFunction> task)
        {
            MetreCount = Convert.ToInt32(metreCount);
            MetreValue = Convert.ToInt32(metreValue);
            SharpsCount = sharpsCount;
            FlatsCount = flatsCount;
            Task = task ?? [];
            Minor = minor;
            Question = question;
        }
    }
}
