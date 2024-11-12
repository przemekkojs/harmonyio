namespace Algorithm.New.Algorithm.Mistake.Solution
{
    public abstract class Mistake
    {
        public const string UNKNOWN_MISTAKE = "Niesprecyzowany błąd";

        public (List<int>, List<int>, string) Description { get; protected set; } = ([], [], UNKNOWN_MISTAKE);
        public abstract int Quantity { get; }

        public abstract void GenerateDescription();

        public static (List<int>, List<int>, string) GenerateStackMistakeDescription(List<int> barIndexes, List<int> verticalIndexes, string ruleName)
        {
            if (barIndexes.Count == 0 || verticalIndexes.Count == 0 || ruleName == null)
                return ([], [], UNKNOWN_MISTAKE);

            return (barIndexes, verticalIndexes, $"Niespełniona zasada: {ruleName}.");
        }

        public static (List<int>, List<int>, string) GenerateNoteMistakeDescription(string voice, int barIndex, int verticalIndex)
        {
            var result = voice == string.Empty ?
                $"Brakujący głos w funkcji." :
                $"Błędny głos {voice} w funkcji.";

            return ([barIndex], [verticalIndex], result);
        }

        public override string ToString() => $"Takty {string.Join(',', Description.Item1)}, Funkcje {string.Join(',', Description.Item2)}, {Description.Item3}.";
    }
}
