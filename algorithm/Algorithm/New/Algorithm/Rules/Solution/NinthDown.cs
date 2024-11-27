using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class NinthDown : Rule
    {
        public NinthDown() : base(
            id: 109,
            name: "Rozwiązanie nony w dół",
            description: "Nona dodana jako składnik dysonujący powinna rozwiązywać się w dół",
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
