using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Parsers.SolutionParser
{
    public class SolutionParseResult
    {
        public Tonation Tonation { get; set; }
        public Metre Meter { get; set; }
        public List<Stack> Stacks { get; set; }

        public SolutionParseResult(Tonation tonation, Metre meter, List<Stack> stacks)
        {
            Tonation = tonation;
            Meter = meter;
            Stacks = stacks;
        }
    }
}
