using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class DeletedEqualsRoot : Rule
    {
        public DeletedEqualsRoot() : base(
            name: "Usunięty składnik równy pozycji",
            description: "",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            throw new NotImplementedException();
        }
    }
}
