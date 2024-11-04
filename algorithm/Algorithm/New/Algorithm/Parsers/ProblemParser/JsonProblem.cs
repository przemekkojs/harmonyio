using Algorithm.New.Music;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Parsers.ProblemParser
{
    public class JsonProblem
    {
        public int MetreValue { get; set; }
        public int MetreCount { get; set; }
        public int SharpsCount { get; set; }
        public int FlatshCount { get; set; }
        public List<Function> Functions { get; set; }

        [JsonConstructor]
        public JsonProblem(int metreCount, int metreValue, int sharpsCount, int flatsCount, List<Function> functions)
        {
            MetreCount = metreCount;
            MetreValue = metreValue;
            SharpsCount = sharpsCount;
            FlatshCount = flatsCount;
            Functions = functions;
        }
    }
}
