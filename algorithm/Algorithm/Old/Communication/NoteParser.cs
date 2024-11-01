using Algorithm.Old.Music;
using Newtonsoft.Json;

namespace Algorithm.Old.Communication
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

    public class NoteParseResult
    {
        public RhytmicValue RhytmicValue { get; private set; }
        public Note Note { get; private set; }
        public int Bar { get; private set; }
        public int Stack { get; private set; }

        public NoteParseResult(Note note, RhytmicValue value, int bar, int stack)
        {
            RhytmicValue = value;
            Note = note;
            Bar = bar;
            Stack = stack;
        }
    }

    public static class NoteParser
    {
        public static NoteParseResult ParseJsonToNote(string jsonString)
        {
            JsonNote? parsedNote = JsonConvert.DeserializeObject<JsonNote>(jsonString) ?? throw new ArgumentException("Parse error.");

            return ParseJsonNoteToNote(parsedNote);
        }

        public static NoteParseResult ParseJsonNoteToNote(JsonNote parsedNote)
        {
            Accidental accidental;

            if (parsedNote.AccidentalName == null || parsedNote.AccidentalName.Length == 0)
                accidental = Accidental.NONE;
            else
            {
                accidental = parsedNote.AccidentalName[1..] switch
                {
                    "#" => Accidental.SHARP,
                    "x" => Accidental.DOUBLE_SHARP,
                    "b" => Accidental.FLAT,
                    "bb" => Accidental.DOUBLE_FLAT,
                    "bq" => Accidental.NEUTRAL,
                    _ => Accidental.NONE
                };
            }

            var (name, octave) = Constants.Constants.NoteMappings[(parsedNote.Line, parsedNote.Voice)];

            Voice voice = parsedNote.Voice switch
            {
                Constants.Constants.SOPRANO => Voice.SOPRANO,
                Constants.Constants.ALTO => Voice.ALTO,
                Constants.Constants.TENORE => Voice.TENORE,
                Constants.Constants.BASS => Voice.BASS,
                _ => throw new ArgumentException("Invalid voice.")
            };

            Note noteResult = new(
                name: name,
                octave: octave,
                voice: voice,
                accidental: accidental,
                neutralized: accidental == Accidental.NEUTRAL
            );

            RhytmicValue rhytmResult = RhytmicValue.GetRhytmicValueByDuration(parsedNote.Value);

            return new NoteParseResult(noteResult, rhytmResult, parsedNote.BarIndex, parsedNote.VerticalIndex);
        }

        public static JsonNote ParseNoteToJsonNote(Note? note, RhytmicValue rhytmicValue, int bar, int stackInBar)
        {
            if (note == null || rhytmicValue == null)
                throw new ArgumentException("Arguments cannot be null.");

            int staff = note.Staff switch
            {
                Staff.UPPER => 1,
                Staff.LOWER => 0,
                _ => throw new ArgumentException("Invalid staff.")
            };

            var matchingNotes = Constants.Constants.NoteMappings
                .Where(kv => kv.Value == (note.Name[0].ToString(), note.Octave))
                .Select(kv => kv.Key)
                .ToList();

            var index = 0;

            while (index < matchingNotes.Count && !matchingNotes[index].Item2.Equals(note.Voice.ToString()))
                index++;

            if (index >= matchingNotes.Count)
                throw new ArgumentException("Invalid voice");

            var line = matchingNotes[index].Item1;

            JsonNote result = new(
                line: line,
                accidentalName: note.Name[1..],
                voice: note.Voice.ToString(),
                value: rhytmicValue.RealDuration,
                barIndex: bar,
                verticalIndex: stackInBar
            );

            return result;
        }

        public static string ParseNoteToJson(Note? note, RhytmicValue rhytmicValue, int bar, int stackInBar)
        {
            var parsed = JsonConvert.SerializeObject(ParseNoteToJson(note, rhytmicValue, bar, stackInBar));
            return parsed.ToString();
        }
    }
}
