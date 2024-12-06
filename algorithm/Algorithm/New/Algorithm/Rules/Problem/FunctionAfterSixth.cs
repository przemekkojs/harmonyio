using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class FunctionAfterSixth : Rule
    {
        public FunctionAfterSixth() : base(
            id: 201,
            name: "Funkcja po sekście",
            description: "Funkcja po sekście musi być oddalona o kwintę w górę.",
            oneFunction: false) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function1 = functions[0];
            var function2 = functions[1];

            var containsSixth = function1.PossibleComponents
                .SelectMany(x => x)
                .Contains(Component.Sixth);
            
            if (containsSixth)
            {
                var index1 = Function.SymbolIndexes[function1.Symbol];
                var index2 = Function.SymbolIndexes[function2.Symbol];

                if (index1 < index2)
                    index2 -= 7;

                var diff = index2 - index1;
                var possibleIndexes = new List<int>() { 4 };
                var possibleContains = possibleIndexes.Contains(diff);

                return possibleContains;
            }

            return true;
        }
    }
}
