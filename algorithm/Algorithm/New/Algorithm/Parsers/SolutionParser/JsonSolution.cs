using Algorithm.New.Algorithm.Parsers.NoteParser;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Parsers.SolutionParser
{
    public class JsonSolution
    {
        [JsonPropertyName("jsonNotes")]
        public List<JsonNote> Notes { get; set; }
        public int SharpsCount { get; set; }
        public int FlatsCount { get; set; }

        [JsonPropertyName("metreCount")]
        public int MetreCount { get; set; }

        [JsonPropertyName("metreValue")]
        public int MetreValue { get; set; }
        public int Minor { get; set; }

        public JsonSolution(List<JsonNote> jsonNotes, int sharpsCount, int flatsCount, int metreValue, int metreCount, int minor)
        {
            Notes = jsonNotes;
            SharpsCount = sharpsCount;
            FlatsCount = flatsCount;
            MetreCount = metreCount;
            MetreValue = metreValue;
            Minor = minor;
        }
    }
}
