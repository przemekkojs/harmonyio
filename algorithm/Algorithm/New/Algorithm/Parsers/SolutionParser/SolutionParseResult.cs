using Algorithm.New.Music;
namespace Algorithm.New.Algorithm.Parsers.SolutionParser
{
    public class TaskParseResult
    {
        public Tonation Tonation { get; set; }
        public Metre Meter { get; set; }
        public List<Stack> Bars { get; set; }

        public TaskParseResult(Tonation tonation, Metre meter, List<Stack> bars)
        {
            Tonation = tonation;
            Meter = meter;
            Bars = bars;
        }
    }
}
