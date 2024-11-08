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

            var notes = parsedTask.Notes;

            // (Bar index, vertical index) : Notes
            Dictionary<(int, int), List<JsonNote>> noteVerticals = new();
            List<Stack> stacks = [];

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

                var soprano = GetNoteByVoice(Constants.SOPRANO, noteList);
                var alto = GetNoteByVoice(Constants.ALTO, noteList);
                var tenore = GetNoteByVoice(Constants.TENORE, noteList);
                var bass = GetNoteByVoice(Constants.BASS, noteList);

                var parsedSoprano = NoteParser.Parser.ParseJsonNoteToNote(soprano);
                var parserAlto = NoteParser.Parser.ParseJsonNoteToNote(alto);
                var parsedTenore = NoteParser.Parser.ParseJsonNoteToNote(tenore);
                var parsedBass = NoteParser.Parser.ParseJsonNoteToNote(bass);

                var toAdd = new Stack(
                    new Music.Index()
                    {
                        Bar = key.Item1,
                        Position = key.Item2,
                        Duration = noteList[0].Value
                    },

                    parsedSoprano?.Note,
                    parserAlto?.Note,
                    parsedTenore?.Note,
                    parsedBass?.Note
                );

                stacks.Add(toAdd);
            }

            var tonation = Tonation.GetTonation(parsedTask.SharpsCount, parsedTask.FlatsCount);
            
            try
            {
                var metre = Metre.GetMetre(parsedTask.MetreCount, parsedTask.MetreValue);
                return new SolutionParseResult(tonation, metre, stacks);
            }
            catch (ArgumentException)
            {
                return new SolutionParseResult(null, null, stacks);
            }
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

        public static string ParseSolutionToJson(Solution solution)
        {
            return "";
        }
    }
}
