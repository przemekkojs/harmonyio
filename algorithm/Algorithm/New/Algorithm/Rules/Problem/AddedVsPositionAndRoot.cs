using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class AddedVsPositionAndRoot : Rule
    {
        public AddedVsPositionAndRoot() : base(
            id: 207,
            name: "Składniki dodane a pozycja i oparcie",
            description: "Pozycja i oparcie musi być zgodna ze składnikami dodanymi",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];
            var added = function.Added;
            var root = function.Root;
            var position = function.Position;

            if (root == null && position == null)
                return true;

            List<Component> rootsToCheck = [Component.Sixth, Component.Seventh];
            List<Component> positionsToCheck =
                [Component.Sixth, Component.Seventh, Component.Ninth];

            if (root != null)
            {
                foreach (var rootToCheck in rootsToCheck)
                {
                    if (root.Equals(rootToCheck))
                    {
                        if (!added.Contains(rootToCheck))
                            return false;
                    }
                }
            }

            if (position != null)
            {
                foreach(var positionToCheck in positionsToCheck)
                {
                    if (position.Equals(positionToCheck))
                    {
                        if (!added.Contains(positionToCheck))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
