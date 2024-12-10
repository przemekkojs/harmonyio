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
            oneFunction: false)
        { }

        // Korzysta z komponentów funkcji
        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var stack1 = stacks[0];
            var stack2 = stacks[1];

            // TODO: Sprawdzenie, czy w ogóle jest dominanta

            var notes1 = stack1.Notes;
            var notes2 = stack2.Notes;

            for (int noteIndex = 0; noteIndex < notes1.Count; noteIndex++)
            {
                var note1 = notes1[noteIndex];
                var note2 = notes2[noteIndex];

                if (note1 == null || note2 == null)
                    continue;

                var component1 = note1.Component;
                var component2 = note2.Component;

                if (component1 == null || component2 == null)
                    continue;

                if (component1.Type != ComponentType.Seventh)
                    continue;

                var resolvedDown = (component2.Type == ComponentType.Third);
                var resolvedUp = (component2.Type == ComponentType.Root);

                if (!resolvedDown && !resolvedUp)
                    return false;
            }

            return true;
        }
    }
}
