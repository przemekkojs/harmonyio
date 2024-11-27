using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    // Tutaj trzeba pamiętać, że mogą być 2 septymy, wtedy jedna w dół, a druga w górę
    public sealed class SeventhDown : Rule
    {
        public SeventhDown() : base(
            id: 110,
            name: "Rozwiązanie septymy w dół",
            description: "Septyma powinna być rozwiązywana w dół.",
            oneFunction: false) { }

        // Korzysta z komponentów funkcji
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
