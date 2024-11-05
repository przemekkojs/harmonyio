using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Parsers.NoteParser
{
    public class JsonNote
    {
        public float Line { get; set; }
        public string AccidentalName { get; set; }
        public string Voice { get; set; }
        public int Value { get; set; }
        public int BarIndex { get; set; }
        public int VerticalIndex { get; set; }

        [JsonConstructor]
        public JsonNote(float line, string accidentalName, string voice, int value, int barIndex, int verticalIndex)
        {
            Line = line;
            AccidentalName = accidentalName;
            Voice = voice;
            Value = value;
            BarIndex = barIndex;
            VerticalIndex = verticalIndex;
        }
    }
}
