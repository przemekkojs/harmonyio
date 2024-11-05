using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules
{
    public class ParallelFifths : Rule
    {
        public ParallelFifths() : base(
            name: "Kwinty równoległe",
            description: "Czy w ramach dwóch funkcji, dwa dowolne głowy poruszają się równolegle do siebie, w interwale kwinty?") { }

        public override bool IsSatisfied(params Stack[] functions)
        {
            throw new NotImplementedException();
        }
    }
}