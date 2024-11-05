using Algorithm.New.Music;

namespace Algorithm.New.Utils
{
    public static class Converters
    {
        public static string ComponentToNote(Component component, Function function)
        {
            var functionIndex = Function.symbolIndexes[function.Symbol];
            var componentIndexes = Function.FunctionComponentIndexes(functionIndex, function.Tonation);
            var componentIndex = componentIndexes[component];

            return componentIndex;
        }
    }
}
