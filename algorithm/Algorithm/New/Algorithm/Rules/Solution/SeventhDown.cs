using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    // Tutaj trzeba pamiętać, że mogą być 2 septymy, wtedy jedna w dół, a druga w górę
    public sealed class SeventhDown : Rule
    {
        public SeventhDown(int id, string name, string description, bool oneFunction = false) : base(
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
