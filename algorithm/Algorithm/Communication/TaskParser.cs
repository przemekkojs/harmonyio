using Algorithm.Algorithm;
using Algorithm.Music;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ObjectiveC;

namespace Algorithm.Communication
{
    public class JsonTask
    {
        [JsonProperty("jsonNotes")]
        public List<JsonNote> Notes { get; set; }
        public int SharpsCount { get; set; }
        public int FlatsCount { get; set; }
        public int MeterCount { get; set; }
        public int MeterValue { get; set; }

        [JsonConstructor]
        public JsonTask(List<JsonNote> notes, int sharpsCount, int flatsCount, int meterCount, int meterValue)
        {
            Notes = notes;
            SharpsCount = sharpsCount;
            FlatsCount = flatsCount;
            MeterCount = meterCount;
            MeterValue = meterValue;
        }
    }

    public class TaskParseResult
    {
        public Tonation Tonation { get; set; }
        public Meter Meter { get; set; }
        public List<UserBar> Bars { get; set; }

        public TaskParseResult(Tonation tonation, Meter meter, List<UserBar> bars)
        {
            Tonation = tonation;
            Meter = meter;
            Bars = bars;
        }
    }

    public static class TaskParser
    {
        public static TaskParseResult ParseJsonToTask(string jsonString, Music.Task baseTask)
        {
            JsonTask? parsedTask = JsonConvert.DeserializeObject<JsonTask>(jsonString) ?? throw new ArgumentException("Invalid JSON string.");

            var tonation = Tonation.GetTonation(parsedTask.SharpsCount, parsedTask.FlatsCount);
            var meter = Meter.GetMeter(parsedTask.MeterValue, parsedTask.MeterCount);

            var bars = new List<UserBar>();

            Dictionary<(int, int), int> beatMappings = [];

            /* 
             * 1. Add Bars
             * foreach (note in parsed.Notes)
             *      look for barIndex - add that many bars how many barIndex-es
             * 
             * 2. Add stacks
             * foreach (note in parsed.Notes)
             *      look for verticalIndex - add that many stacks to each bar...
             *      
             * 3. Add notes to stacks
             * foreach (note in parsed.Notes)
             *      bar[note.barIndex].Stacks[bar.stackIndex].Set<Voice>(...)
             */

            foreach (var note in parsedTask.Notes)
            {
                if (note.BarIndex >= bars.Count)
                    bars.Add(new UserBar());

                if (beatMappings.TryGetValue((note.BarIndex, note.VerticalIndex - 1), out int toSet))
                    toSet = 0;

                beatMappings[(note.BarIndex, note.VerticalIndex)] = toSet;
            }

            var functionsInBars = baseTask.Bars
                .Select(x => x.Functions)
                .ToList();

            for(int barIndex = 0; barIndex < functionsInBars.Count; barIndex++)
            {
                for (int verticalIndex = 0; verticalIndex < functionsInBars[barIndex].Count; verticalIndex++)
                {
                    bars[barIndex].AddStack(new UserStack(
                            functionsInBars[barIndex][verticalIndex],
                            tonation,
                            beatMappings[(barIndex, verticalIndex)]
                    ));
                }
            }

            foreach (var note in parsedTask.Notes)
            {
                var barIndex = note.BarIndex;
                var verticalIndex = note.VerticalIndex;
                var toAdd = NoteParser.ParseJsonNoteToNote(note);
                var userStack = bars[barIndex].UserStacks[verticalIndex];

                switch (toAdd.Note.Voice)
                {
                    case Voice.SOPRANO:
                        userStack.SetSoprano(toAdd.Note, toAdd.RhytmicValue);
                        break;
                    case Voice.ALTO:
                        userStack.SetAlto(toAdd.Note, toAdd.RhytmicValue);
                        break;
                    case Voice.TENORE:
                        userStack.SetTenore(toAdd.Note, toAdd.RhytmicValue);
                        break;
                    default:
                        userStack.SetBass(toAdd.Note, toAdd.RhytmicValue);
                        break;
                }
            }

            return new TaskParseResult(tonation, meter, bars);
        }

        public static string ParseTaskToJson (List<UserBar> task, Tonation tonation, Meter meter)
        {
            int meterValue = meter.Value;
            int meterCount = meter.Count;

            int sharpsCount = tonation.NumberOfSharps;
            int flatsCount = tonation.NumberOfFlats;

            List<JsonNote> jsonNotes = [];

            int barIndex = 0;
            int stackIndex = 0;

            foreach (var userBar in task)
            {
                foreach (var userStack in userBar.UserStacks)
                {
                    foreach (var note in userStack.Notes)
                    {
                        if (note == null)
                            continue;

                        var rhytmicValue = RhytmicValue.GetRhytmicValueByDuration(userStack.Duration);
                        var toAdd = NoteParser.ParseNoteToJsonNote(note, rhytmicValue, barIndex, stackIndex);
                        jsonNotes.Add(toAdd);
                    }

                    stackIndex++;
                }

                barIndex++;
                stackIndex = 0;
            }

            object toParse = new {
                meterValue,
                meterCount,
                sharpsCount,
                flatsCount,
                jsonNotes
            };

            string parsed = JsonConvert.SerializeObject(toParse);
            return parsed;
        }
    }
}
