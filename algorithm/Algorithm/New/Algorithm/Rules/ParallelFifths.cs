using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules
{
    public class ParallelFifths : Rule
    {
        public ParallelFifths() : base(
            name: "Kwinty równoległe",
            description: "Czy w ramach dwóch funkcji, dwa dowolne głowy poruszają się równolegle do siebie, w interwale kwinty?") { }

        public override bool IsSatisfied(params Stack[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var stack1 = functions[0];
            var stack2 = functions[1];

            List<((Note?, Note?), (Note ?, Note ?))> pairs = [
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

                var pair1Interval = Interval.SemitonesBetween(pair1.Item1, pair1.Item2);
                var pair2Interval = Interval.SemitonesBetween(pair2.Item1, pair2.Item2);

                if (pair1Interval == 7 && pair2Interval == 7)
                    return false;
            }

            return true;
        }
    }
}