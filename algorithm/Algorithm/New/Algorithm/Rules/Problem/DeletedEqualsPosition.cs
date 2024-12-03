using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class DeletedEqualsPosition : Rule
    {
        public DeletedEqualsPosition() : base(
            name: "Usunięty składnik równy pozycji",
            description: "",
            oneFunction: false) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            throw new NotImplementedException();
        }
    }
}
