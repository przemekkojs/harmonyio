using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class AddedComponents : Rule
    {
        public AddedComponents() : base(
            id: 209,
            name: "Poprawne składniki dodane",
            description: "Wszystkie ograniczenia narzucone na składniki dodane.",
            oneFunction: true)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];
            var added = function.Added;
            var addedCount = added.Count;

            // Jak nie ma składników dodanych to są dobrze
            if (added == null || addedCount == 0)
                return true;

            // Mogą być max 2
            if (addedCount > 2)
                return false;

            var containsSixth = added.Contains(Component.Sixth);
            var containsSeventh = added.Contains(Component.Seventh);
            var containsNinth = added.Contains(Component.Ninth);

            if (containsSixth)
            {
                if (addedCount > 2)
                    return false;
                else if (containsNinth)
                    return false;
            }

            return true;
        }
    }
}
