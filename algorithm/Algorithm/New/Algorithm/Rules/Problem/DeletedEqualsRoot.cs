using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class DeletedEqualsRoot : Rule
    {
        public DeletedEqualsRoot() : base(
            id: 205,
            name: "Usunięty składnik równy pozycji",
            description: "",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];

            var removed = function.Removed;
            var root = function.Root;

            if (removed == null || root == null)
                return true;

            if (removed.Equals(root))
                return false;

            return true;
        }
    }
}
