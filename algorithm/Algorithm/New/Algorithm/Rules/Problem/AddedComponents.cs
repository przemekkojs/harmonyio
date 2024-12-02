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
            if (!ValidateParametersCount())
                return false;

            var function = functions[0];
            var added = function.Added;

            // Jak nie ma składników dodanych to są dobrze
            if (added == null || added.Count == 0)
                return true;

            var containsSixth = added.Contains(Component.Sixth);
            var containsSeventh = added.Contains(Component.Seventh);
            var containsNinth = added.Contains(Component.Ninth);

            return true;
        }
    }
}
