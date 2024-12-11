using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class RootEqualsPosition : Rule
    {
        public RootEqualsPosition() : base(
            id: 212,
            name: "Oparcie i pozycja ze składnikami dodanymi",
            description: "Oparcie i pozycja nie mogą być takie same przy funkcjach z dodanymi składnikami dysonującymi i żadnym usuniętym",
            oneFunction: true)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];

            var position = function.Position;
            var root = function.Root;
            var removed = function.Removed;
            var added = function.Added;

            if (position != null && root != null)
            {
                var addedCount = added.Count;
                var rootInAdded = added.Contains(root);
                var positionInAdded = added.Contains(position);

                if (addedCount == 1)
                {
                    if (rootInAdded || positionInAdded)
                        return true;

                    if (position.Equals(root))
                        return false;
                }
                else if (addedCount == 2)
                    return (positionInAdded && rootInAdded);
            }

            return true;
        }
    }
}
