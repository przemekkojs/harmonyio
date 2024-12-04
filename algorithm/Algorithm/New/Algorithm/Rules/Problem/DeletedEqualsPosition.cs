using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class DeletedEqualsPosition : Rule
    {
        public DeletedEqualsPosition() : base(
            name: "Usunięty składnik równy pozycji",
            description: "",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];

            var removed = function.Removed;
            var position = function.Position;

            if (removed == null || position == null)
                return true;

            if (removed.Equals(position))
                return false;

            return true;
        }
    }
}
