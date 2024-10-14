using Algorithm.Algorithm;
using Algorithm.Music;
using Newtonsoft.Json;

namespace Algorithm.Communication
{
    public class ParsedTask
    {
        public List<ParsedFunction> Functions { get; set; }
        public string TonationName { get; set; }
        public string TonationMode { get; set; }
        public int MeterCount { get; set; }
        public int MeterValue { get; set; }

        public ParsedTask(List<ParsedFunction> functions, string tonationName, string tonationMode, int meterCount, int meterValue)
        {
            Functions = functions;
            TonationName = tonationName;
            TonationMode = tonationMode;
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
        public static TaskParseResult ParseJsonToTask(string jsonString)
        {
            ParsedTask? parsedTask = JsonConvert.DeserializeObject<ParsedTask>(jsonString) ?? throw new ArgumentException("Invalid JSON string.");

            var tonation = Tonation.GetTonation(parsedTask.TonationName, parsedTask.TonationMode);
            var meter = Meter.GetMeter(parsedTask.MeterValue, parsedTask.MeterCount);

            var bars = new List<UserBar>();

            // TODO: Bars

            return new TaskParseResult(tonation, meter, bars);
        }

        public static string ParseTaskToJson (List<UserBar> task)
        {
            // TODO: Parsing into ParsedTask logic

            string parsed = JsonConvert.SerializeObject(task);
            return parsed;
        }
    }
}
