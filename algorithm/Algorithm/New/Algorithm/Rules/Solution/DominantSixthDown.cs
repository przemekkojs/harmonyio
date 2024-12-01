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

        // Korzysta z komponentów funkcji
        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var function1 = functions[0];

            if (function1.Symbol != Symbol.D)
                return true;

            var stack1 = stacks[0];
            var stack2 = stacks[1];            

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

                if (component1.Type != ComponentType.Sixth)
                    continue;

                var resolvedDown = (component2.Type == ComponentType.Root);
                var resolvedUp = (component2.Type == ComponentType.Third);

                if (!resolvedDown && !resolvedUp)
                    return false;
            }

            return true;
        }
    }
}
