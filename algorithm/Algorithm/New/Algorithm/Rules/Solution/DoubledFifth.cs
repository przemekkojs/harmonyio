using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class DoubledFifth : Rule
    {
        public DoubledFifth() : base(
            id: 119,
            name: "Dwojenie kwinty (funkcje poboczne)",
            description: "W funkcjach pobocznych nie wolno dwoić kwinty",
            oneFunction: true)
        { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            var function = functions[0];
            var functionSymbol = function.Symbol;

            List<Symbol> matchingSymbols =
            [
                Symbol.Sii, Symbol.Tiii, Symbol.Diii, Symbol.Tvi,
                Symbol.Svi, Symbol.Dvii, Symbol.Svi
            ];

            var contains = matchingSymbols.Contains(functionSymbol);

            if (contains)
            {
                var stack = stacks[0];
                var components = stack.Notes
                    .Where(x => x != null)
                    .Select(x => x?.Component);

                var fifths = components   
                    .Where(x => x != null)
                    .Where(x => x.Equals(Component.Fifth));

                var fifthsCount = fifths.Count();

                return fifthsCount == 1;
            }

            return true;
        }
    }
}
