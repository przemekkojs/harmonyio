namespace Algorithm.New.Algorithm.Mistake.Solution
{
    public abstract class Mistake
    {
        public const string UNKNOWN_MISTAKE = "Niesprecyzowany błąd";
        public const string MISSING_VOICE = "Brakujący głos";
        public const string EMPTY_SOLUTION = "Puste rozwiązanie";
        public const string GRADING_MISTAKE = "Nie udało ocenić się tego zadania";

        public const int NO_SOLUTION_CODE = 999;
        public const int GRADING_MISTAKE_CODE = 1000;
        public const int MISSING_VOICE_CODE = -1;

        public int MistakeCode { get; set; }
        public abstract int Quantity { get; }

        public (List<int>, List<int>, int) Description = ([], [], -1);

        public static readonly Dictionary<int, string> VoiceCodes = new()
        {
            { 0, "Sopran" },
            { 1, "Alt" },
            { 2, "Tenor" },
            { 3, "Bas" },
        };

        public static string MistakeCodeToDescription(int mistakeCode)
        {
            switch (mistakeCode)
            {
                case NO_SOLUTION_CODE:
                    return EMPTY_SOLUTION;
                case MISSING_VOICE_CODE:
                    return MISSING_VOICE;
                case GRADING_MISTAKE_CODE:
                    return GRADING_MISTAKE;
                default:
                    break;
            }

            if (VoiceCodes.TryGetValue(mistakeCode, out string? voice))
                return $"Błędny {voice}";
            else
            {
                var rule = Constants.Settings.ActiveRules
                    .FirstOrDefault(r => r.Id == mistakeCode);

                if (rule == null)
                    return UNKNOWN_MISTAKE;

                var ruleName = rule.Name;
                var description = rule.Description;

                return ruleName != null ?
                    $"Niespełniona zasada <span title=\"{description}\" style=\"cursor: pointer;\"><i>{ruleName}</i></span>" :
                    UNKNOWN_MISTAKE;
            }
        }

        public abstract void GenerateDescription();
    }
}
