using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class AllComponentsSatisfied : Rule
    {
        public AllComponentsSatisfied() : base(
            id: 116,
            name: "",
            description: "",
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
