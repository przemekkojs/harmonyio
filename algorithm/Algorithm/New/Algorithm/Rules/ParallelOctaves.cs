using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules
{
    public class ParallelOctaves : Rule
    {
        public ParallelOctaves() : base(
            name: "Oktawy równoległe",
            description: "Czy w ramach dwóch funkcji, dwa dowolne głowy poruszają się równolegle do siebie, w interwale oktawy?") { }

        public override bool IsSatisfied(params Stack[] functions)
        {
            throw new NotImplementedException();
        }
    }
}
