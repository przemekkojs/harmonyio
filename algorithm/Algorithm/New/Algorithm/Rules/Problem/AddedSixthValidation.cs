using Algorithm.New.Music;
using System.Linq;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class AddedSixthValidation : Rule
    {
        public AddedSixthValidation() : base(
            name: "Seksta w funkcjach głównych",
            description: "Dodana seksta może wystąpić jedynie w funkcjach głównych.",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];
            var functionSymbol = function.Symbol;
            var goodSymbols = new List<Symbol>() { Symbol.T, Symbol.S, Symbol.D };
            var symbolInSet = goodSymbols.Contains(functionSymbol);

            if (!symbolInSet)
                return false;

            return true;
        }
    }
}
