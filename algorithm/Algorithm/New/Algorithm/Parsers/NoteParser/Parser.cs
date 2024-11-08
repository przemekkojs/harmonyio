using Algorithm.New.Music;
using Newtonsoft.Json;

namespace Algorithm.New.Algorithm.Parsers.NoteParser
{
    public static class Parser
    {
        public static NoteParseResult? ParseJsonToNote(string jsonString)
        {
            JsonNote? parsedNote = JsonConvert
                .DeserializeObject<JsonNote>(jsonString) ??
                throw new ArgumentException("Parse error.");

            return ParseJsonNoteToNote(parsedNote);
        }

        public static NoteParseResult? ParseJsonNoteToNote(JsonNote? parsedNote)
        {
            if (parsedNote == null)
                return null;

            var accidental = parsedNote.AccidentalName;
            var (name, octave) = Constants.NoteMappings[(parsedNote.Line, parsedNote.Voice)];

            Note noteResult = new(
                name: name,
                octave: octave,
                accidental: accidental
            );

            return new NoteParseResult(noteResult, parsedNote.Value, parsedNote.BarIndex, parsedNote.VerticalIndex);
        }

        public static JsonNote? ParseNoteToJsonNote(Note? note, string voice, int rhytmicValue, int bar, int stackInBar)
        {
            if (note == null)
                return null;

            if (rhytmicValue <= 0)
                throw new ArgumentException("Arguments cannot be 0.");

            // Ta zmienna zwraca wszystkie nuty, które mają określoną wysokość - są na danej linii. Np. nuta G4 może należeć
            // do altu i sopranu jednocześnie.
            var matchingNotes = Constants.NoteMappings
                .Where(kv => kv.Value == (note.Name[0].ToString(), note.Octave))
                .Select(kv => kv.Key)
                .ToList();

            // A w poniższym kodzie ta wartość jest filtrowana, żeby dostać tylko nutę z właściwego głosu
            var index = 0;

            while (index < matchingNotes.Count && !matchingNotes[index].Item2.Equals(voice))
                index++;

            if (index >= matchingNotes.Count)
                throw new ArgumentException("Invalid voice");

            var line = matchingNotes[index].Item1;

            JsonNote result = new(
                line: line,
                accidentalName: note.Name[1..],
                voice: voice,
                value: rhytmicValue,
                barIndex: bar,
                verticalIndex: stackInBar
            );

            return result;
        }

        public static string ParseNoteToJson(Note? note, string voice, int rhytmicValue, int bar, int stackInBar)
        {
            var jsonNote = ParseNoteToJsonNote(note, voice, rhytmicValue, bar, stackInBar);
            var parsed = JsonConvert.SerializeObject(jsonNote);
            return parsed.ToString();
        }
    }
}
