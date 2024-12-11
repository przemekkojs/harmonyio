using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class AllComponentsSatisfied : Rule
    {
        public AllComponentsSatisfied() : base(
            id: 116,
            name: "Wymagane składniki",
            description: "W każdej funkcji wymagana jest minimum pryma albo kwinta i tercja",
            oneFunction: true)
        { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            var stack = stacks[0];
            var stackNotes = stack.Notes;

            var notNullNotes = stackNotes
                .Where(x => x != null)
                .ToList();

            if (notNullNotes == null)
                return true;

            if (notNullNotes.Count != 4)
                return true;

            var stackComponents = notNullNotes
                .Select(x => x!.Component);

            var rootCount = stackComponents
                .Where(x => x!.Equals(Component.Root))
                .Count();

            var fifthCount = stackComponents
                .Where(x => x!.Equals(Component.Fifth))
                .Count();

            var thirdCount = stackComponents
                .Where(x => x!.Equals(Component.Third))
                .Count();

            return thirdCount != 0 && (fifthCount != 0 || rootCount != 0);
        }
    }
}
