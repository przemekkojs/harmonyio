using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    internal class ValidRoot : Rule
    {
        public ValidRoot() : base(
            id: 115,
            name: "Poprawne oparcie",
            description: "Czy oparcie rozwiązania jest zgodne z oparciem problemu",
            oneFunction: true) { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            return true;
        }
    }
}
