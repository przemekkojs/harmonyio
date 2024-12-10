using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class SubdominantAfterDominant : Rule
    {
        public SubdominantAfterDominant() : base(
            id: 200,
            name: "Subdominanta po dominancie",
            description: "Następstwo dowolnej subdominanty nie jest dozwolone po dowolnej dominancie.",
            oneFunction: false)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function1 = functions[0];
            var function2 = functions[1];

            var symbol1 = function1.Symbol.ToString();
            var symbol2 = function2.Symbol.ToString();

            if (!symbol1.Equals("D"))
                return true;

            if (function1.Added.Contains(Component.Sixth))
                return true;

            var symbol1Type = symbol1[0];
            var symbol2Type = symbol2[0];

            if (!function1.Minor)
                return symbol2Type.Equals('T') || symbol2Type.Equals('D');
            else
                return true;
        }
    }
}
