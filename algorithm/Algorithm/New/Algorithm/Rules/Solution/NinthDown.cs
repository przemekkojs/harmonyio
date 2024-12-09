using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class NinthDown : Rule
    {
        public NinthDown() : base(
            id: 109,
            name: "Rozwiązanie nony w dół",
            description: "Nona dodana jako składnik dysonujący powinna rozwiązywać się w dół",
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

            var stack1Notes = stack1.Notes;
            var stack2Notes = stack2.Notes;

            for (int i = 0; i < stack1Notes.Count; i++)
            {
                var component1 = stack1Notes[i]?.Component ?? null;
                var component2 = stack2Notes[i]?.Component ?? null;

                if (component1 == null || component2 == null)
                    continue;

                if (component1.Type == ComponentType.Ninth &&
                    component2.Type != ComponentType.Fifth)
                    return false;
            }

            return true;
        }
    }
}
