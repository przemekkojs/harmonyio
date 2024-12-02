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

            return result;
        }

        public static bool Contains(List<string> toCheck, List<List<string>> possible)
        {
            foreach(var possibleNotes in possible)
            {
                for (int index = 0; index < possibleNotes.Count; index++)
                {
                    if (!toCheck[index].Equals(possibleNotes[index]))
                        return false;
                }
            }

            return true;
        }
    }
}
