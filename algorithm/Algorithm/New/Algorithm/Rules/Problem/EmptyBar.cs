using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class EmptyBar : Rule
    {
        public EmptyBar() : base(
            id: 204,
            name: "Pusty takt",
            description: "Nie można mieć pustych taktów w zadaniu",
            oneFunction: false)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var barIndex1 = functions[0].Index.Bar;
            var barIndex2 = functions[1].Index.Bar;

            var diff = Math.Abs(barIndex2 - barIndex1);

            if (diff > 1)
                return false;

            return true;
        }
    }
}
