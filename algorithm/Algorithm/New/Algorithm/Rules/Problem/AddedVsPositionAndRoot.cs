using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class AddedVsPositionAndRoot : Rule
    {
        public AddedVsPositionAndRoot() : base(
            name: "Składniki dodane a pozycja i oparcie",
            description: "Pozycja i oparcie musi być zgodna ze składnikami dodanymi",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            return true;
        }
    }
}
