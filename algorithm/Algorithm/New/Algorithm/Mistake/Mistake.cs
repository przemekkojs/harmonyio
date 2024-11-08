namespace Algorithm.New.Algorithm.Mistake
{
    public abstract class Mistake
    {
        public string Description { get; protected set; } = "Niesprecyzowany błąd";
        public abstract int Quantity { get; }

        public abstract void GenerateDescription();

        public static string GenerateStackMistakeDescription(List<int> barIndexes, List<int> verticalIndexes, string ruleName)
        {
            if (barIndexes.Count == 0 || verticalIndexes.Count == 0 || ruleName == null)
                return "";

            var count = verticalIndexes.Count;
            var postfix = count == 1 ? "i" : "ach";
            var result = $"Błąd w funkcj{postfix}: ";

            if (count == 1)
                result += $"Takt: {barIndexes[0]}, Miara: {verticalIndexes[0]}. Niespełniona zasada: {ruleName}.";
            else
            {
                var barCount = barIndexes.Count;

                if (barCount == 1)
                    result += $"Takt {barIndexes[0]}, Funkcje {verticalIndexes[0]}, {verticalIndexes[1]}. Niespełniona zasada {ruleName}.";
                else
                    result += $"(Takt {barIndexes[0]}, Funkcja {verticalIndexes[0]}), (Takt {barIndexes[1]}, Funkcja {verticalIndexes[1]}). Niespełniona zasada {ruleName}.";
            }
                

            return result;
        }

        public static string GenerateNoteMistakeDescription(string voice, int barIndex, int verticalIndex)
        {
            return voice == string.Empty ?
                $"Brakujący głos w funkcji: Takt: {barIndex}, Funkcja: {verticalIndex}." : 
                $"Błędny głos {voice} w funkcji: Takt: {barIndex}, Funkcja: {verticalIndex}.";
        }

        public override string ToString() => Description;
    }
}
