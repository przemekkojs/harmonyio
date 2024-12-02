using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class AddedComponents : Rule
    {
        public AddedComponents() : base(
            name: "Poprawne składniki dodane",
            description: "Wszystkie ograniczenia narzucone na składniki dodane.",
            oneFunction: true) { }

        public override bool IsSatisfied(params Function[] functions)
        {
            throw new NotImplementedException();
        }
    }
}
