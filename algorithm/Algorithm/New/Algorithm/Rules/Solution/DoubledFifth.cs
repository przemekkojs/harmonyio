using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class DoubledFifth : Rule
    {
        public DoubledFifth() : base(
            id: 119,
            name: "Dwojenie kwinty (funkcje poboczne)",
            description: "W funkcjach pobocznych nie wolno dwoić kwinty",
            oneFunction: true) { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            return true;
        }
    }
}
