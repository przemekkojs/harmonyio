using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class DominantThirdUp : Rule
    {
        public DominantThirdUp() : base(
            id: 106,
            name: "Rozwiązanie tercji dominanty",
            description: "",
            oneFunction: false) { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            throw new NotImplementedException();
        }
    }
}
