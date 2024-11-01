using Algorithm.New.Music;

namespace Algorithm.New.Algorithm
{
    public class Problem
    {
        public List<Function> Functions {  get; private set; }
        public Metre Metre { get; private set; }
        public Tonation Tonation { get; private set; }

        public Problem(List<Function> functions, Metre metre, Tonation tonation)
        {
            Functions = functions;
            Metre = metre;
            Tonation = tonation;
        }

        public Problem(Metre metre, Tonation tonation) : this([], metre, tonation) { }
    }
}
