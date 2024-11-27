using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class DominantThirdUp : Rule
    {
        public DominantThirdUp() : base(
            id: 106,
            name: "Rozwiązanie tercji dominanty",
            description: "Tercja dominanty powinna się rozwiązywać w górę",
            oneFunction: false) { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            return true;
        }
    }
}
