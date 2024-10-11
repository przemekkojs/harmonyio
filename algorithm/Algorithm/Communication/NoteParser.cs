using Algorithm.Music;
using Newtonsoft.Json;

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
    }

    public static class NoteParser
    {
        public static (Note, RhytmicValue) ParseJsonToNote(string jsonString)
        {
            object? parsed = JsonConvert.DeserializeObject(jsonString) ?? throw new ArgumentException("Parse error.");

            if (parsed is not ParsedNote parsedNote)
                throw new ArgumentException("Parse error.");

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

            return (noteResult, rhytmResult);
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

            int voice = note.Voice switch
            {
                Voice.SOPRANO => 0,
                Voice.ALTO => 1,
                Voice.TENORE => 2,
                Voice.BASS => 3,
                _ => throw new ArgumentException("Invalid voice.")
            };

            ParsedNote toParse = new(
                name: note.Name,
                octave: note.Octave,
                duration: rhytmicValue.RealDuration,
                bar: bar,
                stack: stackInBar,
                staff: staff,
                voice: voice,
                neutralized: note.Accidental == Accidental.NEUTRAL
            );

            var parsed = JsonConvert.SerializeObject(toParse);
            return parsed.ToString();
        }
    }
}
