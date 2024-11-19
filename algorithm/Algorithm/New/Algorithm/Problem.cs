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

            foreach (var function in Functions)
            {
                function.Tonation = tonation;
            }
        }

        public Problem(Metre metre, Tonation tonation, int maxPoints) : this([], metre, tonation) { }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Problem casted)
            {
                if (casted.Functions.Count != Functions.Count)
                    return false;

                if (!casted.Tonation.Equals(Tonation))
                    return false;

                if (!casted.Metre.Equals(Metre))
                    return false;

                for (int index = 0; index < casted.Functions.Count; index++)
                {
                    var function1 = Functions[index];
                    var function2 = casted.Functions[index];

                    if (function1 != null)
                    {
                        var functionsEqual = function1.Equals(function2);

                        if (!functionsEqual)
                            return false;
                    }
                }
            }
            else
                return false;

            return true;
        }
    }
}
