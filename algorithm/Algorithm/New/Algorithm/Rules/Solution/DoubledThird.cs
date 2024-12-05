using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class DoubledThird : Rule
    {
        public DoubledThird() : base(
            id: 118,
            name: "Dwojenie tercji (funkcje główne)",
            description: "W funkcjach głównych nie wolno dwoić tercji",
            oneFunction: true) { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            throw new NotImplementedException();
        }
    }
}
