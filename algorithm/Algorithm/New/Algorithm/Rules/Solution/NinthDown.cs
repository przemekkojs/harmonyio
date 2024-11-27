using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class NinthDown : Rule
    {
        public NinthDown(int id, string name, string description, bool oneFunction = false) : base(
            id,
            name,
            description,
            oneFunction) { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            throw new NotImplementedException();
        }
    }
}
