using Algorithm.New.Music;

namespace Algorithm.New.Utils
{
    public static class PossibleNotes
    {
        public static List<List<string>> GeneratePossibleNotes(Function function)
        {
            List<List<string>> result = [];

            foreach (var componentSet in function.PossibleComponents)
            {
                List<string> toAdd = [];

                foreach (var component in componentSet)
                {
                    string noteName = Converters.ComponentToNote(component, function);
                    toAdd.Add(noteName);
                }

                result.Add(toAdd);
            }

            return result;
        }
    }
}
