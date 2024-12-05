using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class ParallelOctaves : Rule
    {
        public ParallelOctaves() : base(
            id: 103,
            name: "Oktawy równoległe",
            description: "Czy w ramach dwóch funkcji, dwa dowolne głowy poruszają się równolegle do siebie, w interwale oktawy?")
        { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var stack1 = stacks[0];
            var stack2 = stacks[1];

            List<((Note?, Note?), (Note?, Note?))> pairs = [
                ((stack1.Soprano, stack1.Alto), (stack2.Soprano, stack2.Alto)),
                ((stack1.Soprano, stack1.Tenore), (stack2.Soprano, stack2.Tenore)),
                ((stack1.Soprano, stack1.Bass), (stack2.Soprano, stack2.Bass)),
                ((stack1.Alto, stack1.Tenore), (stack2.Alto, stack2.Tenore)),
                ((stack1.Alto, stack1.Bass), (stack2.Alto, stack2.Bass)),
                ((stack1.Tenore, stack1.Bass), (stack2.Tenore, stack2.Bass))
            ];

            foreach (var pair in pairs)
            {
                var pair1 = pair.Item1;
                var pair2 = pair.Item2;

                var note1_1 = pair1.Item1;
                var note1_2 = pair1.Item2;
                var note2_1 = pair2.Item1;
                var note2_2 = pair2.Item2;

                var pair1Interval = Interval.SemitonesBetween(note1_1, note1_2);
                var pair2Interval = Interval.SemitonesBetween(note2_1, note2_2);
                var notesSame =
                    Interval.SemitonesBetween(note1_1, note2_1) +
                    Interval.SemitonesBetween(note1_2, note2_2);

                if (new List<Note?>() { note1_1, note1_2, note2_1, note2_2 }.Contains(null))
                    continue;

                // Wsm tu jest jedyna zmiana między tą zasadą a ParallelFifths, trzeba by coś
                // z tym zrobić, ale bardzo nie kłuje w oczy, więc na razie zostawiam
                // Do zobaczenia za 1000 PR-ów XDDDDD
                // 8.11.2024: Na razie jestem na 4., spoko idzie
                if (pair1Interval % 12 == 0 && pair2Interval % 12 == 0&& notesSame != 0)
                    return false;
            }

            return true;
        }
    }
}
