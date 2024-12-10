namespace Algorithm.New.Algorithm.Mistake.Solution
{
    public abstract class Mistake
    {
        public const string UNKNOWN_MISTAKE = "Niesprecyzowany błąd";
        public const string MISSING_VOICE = "Brakujący głos.";
        public const string EMPTY_SOLUTION = "Puste rozwiązanie";

        public int MistakeCode { get; set; }
        public abstract int Quantity { get; }

        public (List<int>, List<int>, int) Description = ([], [], -1);

        public static readonly Dictionary<int, string> VoiceCodes = new()
        {
            { 0, "S" },
            { 1, "A" },
            { 2, "T" },
            { 3, "B" },
        };

        public static string MistakeCodeToDescription(int mistakeCode)
        {
            if (mistakeCode == 999)
                return EMPTY_SOLUTION;

            if (mistakeCode == -1)
                return MISSING_VOICE;

            if (VoiceCodes.TryGetValue(mistakeCode, out string? voice))
                return $"Błędny głos {voice}.";
            else
            {
                return $"Niespełniona zasada {Constants.Settings.ActiveRules
                    .FirstOrDefault(r => r.Id == mistakeCode)?.Name}" ?? UNKNOWN_MISTAKE;
            }
        }

        public abstract void GenerateDescription();
    }
}
