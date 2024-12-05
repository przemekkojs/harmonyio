using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class ValidPosition : Rule
    {
        public ValidPosition() : base(
            id: 114,
            name: "Poprawna pozycja",
            description: "Czy pozycja rozwiązania jest zgodna z pozycją problemu",
            oneFunction: true) { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            var function = functions[0];
            var stack = stacks[0];

            var functionPosition = function.Position;

            if (functionPosition == null)
                return true;

            var stackPosition = stack.Soprano?.Component;

            return (functionPosition.Equals(stackPosition));
        }
    }
}
