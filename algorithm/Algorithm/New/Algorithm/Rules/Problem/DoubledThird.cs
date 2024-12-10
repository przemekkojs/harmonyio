using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class DoubledThird : Rule
    {
        public DoubledThird() : base(
            id: 213,
            name: "Dwojenie tercji",
            description: "W funkcjach głównych nie wolno dwoić tercji. W funkcjach pobocznych należy.",
            oneFunction: true)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];
            var position = function.Position;
            var root = function.Root;
            var isMain = function.IsMain;

            if (position == null || root == null)
                return true;

            if (isMain)
            {
                var positionIsThird = position.Equals(Component.Third);
                var rootIsThird = root.Equals(Component.Third);

                if (positionIsThird && rootIsThird)
                    return false;
            }

            return true;
        }
    }
}
