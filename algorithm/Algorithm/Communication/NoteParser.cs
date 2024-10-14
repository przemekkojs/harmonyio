using Algorithm.Music;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Algorithm.Communication
{
    public class ParsedNote
    {
        public string Name { get; private set; }
        public int Duration { get; private set; }
        public int Octave { get; private set; }
        public int Bar { get; private set; }
        public int Stack { get; private set; }
        public int Staff { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Voice Voice { get; private set; }
        public bool Neutralized { get; private set; }

        public ParsedNote(string name, int octave, int duration, int bar, int stack, int staff, int voice, bool neutralized)
        {
            Name = name;
            Duration = duration;
            Octave = octave;
            Bar = bar;
            Stack = stack;
            Staff = staff;
            Neutralized = neutralized;

            Voice = voice switch
            {
                0 => Voice.SOPRANO,
                1 => Voice.ALTO,
                2 => Voice.TENORE,
                3 => Voice.BASS,
                _ => throw new ArgumentException("Invalid voice")
            };
        }

        public ParsedNote(string name, int octave, int duration, int bar, int stack, int staff, string voice, bool neutralized)
        {
            Name = name;
            Duration = duration;
            Octave = octave;
            Bar = bar;
            Stack = stack;
            Staff = staff;
            Neutralized = neutralized;

            Voice = voice switch
            {
                "SOPRANO" => Voice.SOPRANO,
                "ALTO" => Voice.ALTO,
                "TENORE" => Voice.TENORE,
                "BASS" => Voice.BASS,
                _ => throw new ArgumentException("Invalid voice")
            };
        }

        [JsonConstructor]
        public ParsedNote(string name, int octave, int duration, int bar, int stack, int staff, Voice voice, bool neutralized)
        {
            Name = name;
            Duration = duration;
            Octave = octave;
            Bar = bar;
            Stack = stack;
            Staff = staff;
            Neutralized = neutralized;
            Voice = voice;
        }
    }

    public class ParseResult
    {
        public RhytmicValue RhytmicValue { get; private set; }
        public Note Note { get; private set; }
        public int Bar { get; private set; }
        public int Stack { get; private set; }

        public ParseResult(Note note, RhytmicValue value, int bar, int stack)
        {
            RhytmicValue = value;
            Note = note;
            Bar = bar;
            Stack = stack;
        }
    }

    public static class NoteParser
    {
        public static ParseResult ParseJsonToNote(string jsonString)
        {
            ParsedNote? parsedNote = JsonConvert.DeserializeObject<ParsedNote>(jsonString) ?? throw new ArgumentException("Parse error.");

            Accidental accidental = parsedNote.Name[1..].ToLower() switch
            {
                "" => Accidental.NONE,
                "#" => Accidental.SHARP,
                "x" => Accidental.DOUBLE_SHARP,
                "b" => Accidental.FLAT,
                "bb" => Accidental.DOUBLE_FLAT,
                "bq" => Accidental.NEUTRAL,
                _ => throw new ArgumentException("Invalid accidental.")
            };

            Note noteResult = new(
                name: parsedNote.Name,
                octave: parsedNote.Octave,
                voice: parsedNote.Voice,
                accidental: accidental,
                neutralized: parsedNote.Neutralized
            );

            RhytmicValue rhytmResult = RhytmicValue.GetRhytmicValueByDuration(parsedNote.Duration);

            return new ParseResult(noteResult, rhytmResult, parsedNote.Bar, parsedNote.Stack);
        }

        public static string ParseNoteToJson(Note? note, RhytmicValue rhytmicValue, int bar, int stackInBar)
        {
            if (note == null || rhytmicValue == null)
                throw new ArgumentException("Arguments cannot be null.");

            int staff = note.Staff switch
            {
                Staff.UPPER => 1,
                Staff.LOWER => 0,
                _ => throw new ArgumentException("Invalid staff.")
            };

            ParsedNote toParse = new(
                name: note.Name,
                octave: note.Octave,
                duration: rhytmicValue.RealDuration,
                bar: bar,
                stack: stackInBar,
                staff: staff,
                voice: note.Voice.ToString(),
                neutralized: note.Accidental == Accidental.NEUTRAL
            );

            var parsed = JsonConvert.SerializeObject(toParse);
            return parsed.ToString();
        }
    }
}
