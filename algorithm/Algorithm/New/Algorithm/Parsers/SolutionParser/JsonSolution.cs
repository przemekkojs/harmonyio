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
        public int MeterCount { get; set; }
        public int MeterValue { get; set; }

        [JsonConstructor]
        public JsonSolution(List<JsonNote> notes, int sharpsCount, int flatsCount, int meterCount, int meterValue)
        {
            Notes = notes;
            SharpsCount = sharpsCount;
            FlatsCount = flatsCount;
            MeterCount = meterCount;
            MeterValue = meterValue;
        }
    }
}
