using Algorithm.New.Algorithm.Parsers.NoteParser;
using Algorithm.New.Music;
using Newtonsoft.Json;

namespace Algorithm.New.Algorithm.Parsers.SolutionParser
{
    public static class Parser
    {
        public static SolutionParseResult ParseJsonToSolutionParseResult(string solutionJsonString)
        {
            JsonSolution? parsedTask = JsonConvert.DeserializeObject<JsonSolution>(solutionJsonString) ?? throw new ArgumentException("Invalid JSON string.");

            var minor = parsedTask.Minor != 1;
            var sharpsCount = parsedTask.SharpsCount;
            var flatsCount = parsedTask.FlatsCount;

            var notes = parsedTask.Notes;
            var tonationList = Tonation.GetTonation(sharpsCount, flatsCount);

            var tonation = (minor ?
                tonationList.FirstOrDefault(x => x.Mode == Mode.Minor) :
                tonationList.FirstOrDefault(x => x.Mode == Mode.Major)) ??
                throw new ArgumentException("Invalid tonation");

            var metre = Metre.GetMetre(parsedTask.MetreCount, parsedTask.MetreValue);

            // (Bar index, vertical index) : Notes
            Dictionary<(int, int), List<JsonNote>> noteVerticals = new();
            List<Stack> stacks = [];

            var accidentalNotes = tonation.SharpsCount > 0 ?
                Tonation.SharpsNotes[0..tonation.SharpsCount] :
                Tonation.FlatsNotes[0..tonation.FlatsCount];

            var accidental = tonation.SharpsCount > 0 ?
                "#" : 
                tonation.FlatsCount > 0 ? 
                    "b" :
                    "";

            if (accidentalNotes == null)
                accidentalNotes = [];

            // TODO: Korekta wszystkich nut
            // Wzięcie wszystkich nut
            foreach (var note in notes)
            {
                var barIndex = note.BarIndex;
                var verticalIndex = note.VerticalIndex;
                var key = (barIndex, verticalIndex);

                if (!noteVerticals.ContainsKey(key))
                    noteVerticals[key] = [];

                noteVerticals[key].Add(note);
            }

            // Ustawienie nut w stosach
            foreach (var key in noteVerticals.Keys)
            {
                var noteList = noteVerticals[key] ?? [];

                var sopranoJsonNote = GetNoteByVoice(Constants.SOPRANO, noteList);
                var altoJsonNote = GetNoteByVoice(Constants.ALTO, noteList);
                var tenoreJsonNote = GetNoteByVoice(Constants.TENORE, noteList);
                var bassJsonNote = GetNoteByVoice(Constants.BASS, noteList);

                var parsedSoprano = NoteParser.Parser.ParseJsonNoteToNote(sopranoJsonNote);
                var parserAlto = NoteParser.Parser.ParseJsonNoteToNote(altoJsonNote);
                var parsedTenore = NoteParser.Parser.ParseJsonNoteToNote(tenoreJsonNote);
                var parsedBass = NoteParser.Parser.ParseJsonNoteToNote(bassJsonNote);

                var soprano = parsedSoprano?.Note;
                var alto = parserAlto?.Note;
                var tenore = parsedTenore?.Note;
                var bass = parsedBass?.Note;

                List<Note?> tmpList = [soprano, alto, tenore, bass];

                foreach (var note in tmpList)
                {
                    SetNoteAccidental(note, accidental, accidentalNotes);
                }

                var toAdd = new Stack(
                    new Music.Index()
                    {
                        Bar = key.Item1,
                        Position = key.Item2,
                        Duration = noteList[0].Value
                    },

                    soprano,
                    alto,
                    tenore,
                    bass
                );

                stacks.Add(toAdd);
            }            

            return new SolutionParseResult(tonation, metre, stacks);
        }

        // TODO: Make a one-liner using FirstOrDefault<>()
        private static JsonNote? GetNoteByVoice(string voice, List<JsonNote>? notes)
        {
            if (notes == null)
                return null;

            foreach(var note in notes)
            {
                if (note == null)
                    continue;

                if (note.Voice == voice)
                    return note;
            }

            return null;
        }

        private static void SetNoteAccidental(Note? note, string accidental, List<string> accidentalNotes)
        {
            if (note != null)
            {
                if (accidentalNotes.Contains(note.Name))
                    note.SetNewName(note.Name + accidental);
            }
        }

        public static string ParseSolutionToJson(Solution solution)
        {
            var metre = solution.Problem.Metre;
            var tonation = solution.Problem.Tonation;
            var stacks = solution.Stacks;

            var metreValue = metre.Value;
            var metreCount = metre.Count;

            var sharpsCount = tonation.SharpsCount;
            var flatsCount = tonation.FlatsCount;
            var minor = tonation.Mode == Mode.Minor ?
                0 :
                1;

            List<JsonNote> jsonNotes = [];

            foreach (var stack in stacks)
            {
                var voiceIndex = 0;
                var rhytmicValue = stack.Index.Duration;
                var bar = stack.Index.Bar;
                var vertical = stack.Index.Position;

                foreach (var note in stack.Notes)
                {
                    var voice = voiceIndex switch
                    {
                        0 => Constants.SOPRANO,
                        1 => Constants.ALTO,
                        2 => Constants.TENORE,
                        _ => Constants.BASS
                    };                   

                    var toAdd = NoteParser.Parser
                        .ParseNoteToJsonNote(note, voice, rhytmicValue, bar, vertical);

                    if (toAdd != null)
                        jsonNotes.Add(toAdd);
                }
            }

            var result = new
            {
                metreValue,
                metreCount,
                sharpsCount,
                flatsCount,
                minor,
                jsonNotes
            };

            return JsonConvert.SerializeObject(result);
        }
    }
}
