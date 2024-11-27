using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class AugmentedInterval : Rule
    {
        public AugmentedInterval() : base(
            id: 107,
            name: "Ruch o interwał zwiększony",
            description: "",
            oneFunction: false) { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            throw new NotImplementedException();
        }
    }
}
