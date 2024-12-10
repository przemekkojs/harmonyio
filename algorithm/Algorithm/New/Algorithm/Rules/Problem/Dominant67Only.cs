using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class Dominant67Only : Rule
    {
        public Dominant67Only() : base(
            id: 216,
            name: "Ahord Chopinowski na dominancie",
            description: "Tylko durowa dominanta może mieć dodaną septymę oraz sekstę zamiast kwinty",
            oneFunction: true)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];
            var symbol = function.Symbol;
            var minor = function.Minor;

            var added = function.Added;

            if (added.Count != 2)
                return true;

            if (!(added.Contains(Component.Sixth) && added.Contains(Component.Seventh))
                return true;

            if (symbol != Symbol.D)
                return !minor;

            return true;
        }
    }
}
