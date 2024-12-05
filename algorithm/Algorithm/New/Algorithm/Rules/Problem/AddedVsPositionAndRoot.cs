using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class AddedVsPositionAndRoot : Rule
    {
        public AddedVsPositionAndRoot() : base(
            name: "",
            description: "",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            return true;
        }
    }
}
