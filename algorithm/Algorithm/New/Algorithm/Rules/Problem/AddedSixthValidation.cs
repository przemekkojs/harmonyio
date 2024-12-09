using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class AddedSixthValidation : Rule
    {
        public AddedSixthValidation() : base(
            id: 208,
            name: "Seksta w funkcjach głównych",
            description: "Dodana seksta może wystąpić jedynie w funkcjach głównych.",
            oneFunction: true)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];
            var added = function.Added;

            if (!added.Contains(Component.Sixth))
                return true;

            var functionSymbol = function.Symbol;
            var goodSymbols = new List<Symbol>() { Symbol.T, Symbol.S, Symbol.D };
            var symbolInSet = goodSymbols.Contains(functionSymbol);

            if (!symbolInSet)
                return false;

            return true;
        }
    }
}
