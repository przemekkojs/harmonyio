using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    // Tutaj pamiętać, że dla dominanty jest osobna zasada
    public sealed class SixthUp : Rule
    {
        public SixthUp() : base(
            id: 111,
            name: "Rozwiązanie seksty w górę",
            description: "W funkcjach innych niż dominanty, seksty powinne być rozwiązywane w górę.",
            oneFunction: false) { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var stack1 = stacks[0];
            var stack2 = stacks[1];

            for (int noteIndex = 0; noteIndex < stack1.Notes.Count; noteIndex++)
            {
                var note1 = stack1.Notes[noteIndex];
                var note2 = stack2.Notes[noteIndex];

                // Tutaj jakoś trzeba zrobić mapowanie nuty na komponent
            }

            return true;
        }
    }
}
