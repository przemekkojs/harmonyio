namespace Algorithm.Constants
{
    public static class Constants
    {
        public const int NOTES_IN_FUNCTION = 4;

        public const string WHOLE_NOTE_NAME = "Whole note";
        public const string HALF_NOTE_NAME = "Half note";
        public const string QUARTER_NOTE_NAME = "Quarter note";
        public const string EIGHTH_NOTE_NAME = "Eighth note";
        public const string SIXTEENTH_NOTE_NAME = "Sixteentx note";

        public static readonly List<string> SharpsQueue = ["F", "C", "G", "D", "A", "E", "B"];
        public static readonly List<string> FlatsQueue = ["B", "E", "A", "D", "G", "C", "F"];
        public static readonly List<string> AllNotes = ["C", "D", "E", "F", "G", "A", "B"];
    }
}
