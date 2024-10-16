using Algorithm.Algorithm;
using Algorithm.Music;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ObjectiveC;

namespace Algorithm.Communication
{
    public class JsonTask
    {
        public List<JsonNote> Notes { get; set; }
        public int SharpsCount { get; set; }
        public int FlatsCount { get; set; }
        public int MeterCount { get; set; }
        public int MeterValue { get; set; }

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
            var currentBar = new UserBar();
            UserStack userStack = new (baseTask.Bars[0].Functions[0], tonation, 0);
            var lastBarIndex = 0;
            var currentBeat = 0;
            var lastStackIndex = -1;

            // TODO: Bars
            foreach (var jsonNote in parsedTask.Notes)
            {
                if (jsonNote.BarIndex > lastBarIndex)
                {
                    bars.Add(currentBar);
                    currentBar = new UserBar();
                    lastStackIndex = -1;
                    currentBeat = 0;
                }

                if (jsonNote.VerticalIndex > lastStackIndex)
                {
                    userStack = new UserStack(
                        baseFunction: baseTask.Bars[jsonNote.BarIndex].Functions[jsonNote.VerticalIndex],
                        tonation: tonation,
                        startBeat: currentBeat
                    );

                    currentBeat += jsonNote.Value;
                    lastStackIndex = jsonNote.VerticalIndex;
                }

                var parseResult = NoteParser.ParseJsonNoteToNote(jsonNote);
                
                switch (parseResult.Note.Voice)
                {
                    case Voice.SOPRANO:
                        userStack.SetSoprano(parseResult.Note, parseResult.RhytmicValue);
                        break;
                    case Voice.ALTO:
                        userStack.SetAlto(parseResult.Note, parseResult.RhytmicValue);
                        break;
                    case Voice.TENORE:
                        userStack.SetTenore(parseResult.Note, parseResult.RhytmicValue);
                        break;
                    default:
                        userStack.SetBass(parseResult.Note, parseResult.RhytmicValue);
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
