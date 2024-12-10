using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class DoubledFifth : Rule
    {
        public DoubledFifth() : base(
            id: 214,
            name: "Dwojenie kwinty",
            description: "W funkcjach pobocznych nie wolno dwoić tercji. W funkcjach głównych można.",
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

            if (!isMain)
            {
                var positionIsFifth = position.Equals(Component.Fifth);
                var rootIsFifth = root.Equals(Component.Fifth);

                if (positionIsFifth && rootIsFifth)
                    return false;
            }

            return true;
        }
    }
}
