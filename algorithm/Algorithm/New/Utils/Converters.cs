using Algorithm.New.Music;

namespace Algorithm.New.Utils
{
    public static class Converters
    {
        public static string ComponentToNote(Component component, Function function)
        {
            var minor = function.Minor;
            var tonation = function.Tonation;

            var option1 = function.Tonation.Mode == Mode.Major && minor;
            var option2 = function.Tonation.Mode == Mode.Minor && !minor;

            if (option1 || option2)
                tonation = Tonation.GetMirroredVersion(tonation);

            var functionIndex = Function.symbolIndexes[function.Symbol];
            var componentIndexes = Function.FunctionComponentIndexes(functionIndex, tonation);
            var componentIndex = componentIndexes[component];

            return componentIndex;
        }
    }
}
