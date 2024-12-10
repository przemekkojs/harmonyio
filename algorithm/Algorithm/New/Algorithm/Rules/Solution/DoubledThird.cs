using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class DoubledThird : Rule
    {
        public DoubledThird() : base(
            id: 118,
            name: "Dwojenie tercji (funkcje główne)",
            description: "W funkcjach głównych nie wolno dwoić tercji",
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

            List<Symbol> matchingSymbols = [Symbol.T, Symbol.S, Symbol.D];

            var contains = matchingSymbols.Contains(functionSymbol);

            if (contains)
            {
                var stack = stacks[0];
                var components = stack.Notes
                    .Where(x => x != null)
                    .Select(x => x?.Component);

                var thirds = components
                    .Where(x => x.Equals(Component.Third));

                var thirdsCount = thirds.Count();

                return thirdsCount == 1;
            }

            return true;
        }
    }
}
