using Algorithm.New.Music;

namespace Algorithm.New.Utils
{
    public static class PossibleNotes
    {
        public static List<List<(string, Component)>> GeneratePossibleNotes(Function function)
        {
            List<List<(string, Component)>> result = [];

            foreach (var componentSet in function.PossibleComponents)
            {
                List<(string, Component)> toAdd = [];

                foreach (var component in componentSet)
                {
                    string noteName = Converters.ComponentToNote(component, function);
                    toAdd.Add((noteName, component));
                }

                result.Add(toAdd);
            }

            result = ValidatedResult(function, result);

            if (result.Count == 0)
                throw new Exception("Something went wrong");

            return result;
        }

        private static List<List<(string, Component)>> ValidatedResult(Function function, List<List<(string, Component)>> result)
        {
            List<List<(string, Component)>> toRemove = [];

            var functionPosition = function.Position;
            var functionRoot = function.Root;

            foreach (var possible in result)
            {
                var position = possible[0].Item2;
                var root = possible[3].Item2;

                // Wszystkie, gdzie pozycja się nie zgadza
                if (functionPosition != null && position != null)
                {
                    var equals = functionPosition.Equals(position);

                    if (!equals)
                        toRemove.Add(possible);
                }

                // Wszystkie, gdzie oparcie się nie zgadza
                if (functionRoot != null && root != null)
                {
                    var equals = functionRoot.Equals(root);

                    if (!equals)
                        toRemove.Add(possible);
                }
            }

            if (toRemove.Count > 0)
            {
                return result
                    .Where(x => !toRemove.Contains(x))
                    .ToList();
            }

            return result;
        }
    }
}
