using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class EmptyFunction : Rule
    {
        public EmptyFunction() : base(
            id: 203,
            name: "Pusta funkcja",
            description: "W zadaniu nie może być pustych funkcji",
            oneFunction: true)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];
            var symbol = function.Symbol;

            return symbol != Symbol.None;
        }
    }
}
