using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class DominantSixthDown : Rule
    {
        public DominantSixthDown() : base(
            id: 108,
            name: "Rozwiązanie seksty dominanty w dół",
            description: "Seksta w dominancie powinna być rozwiązywana w dół",
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
