using Algorithm.Algorithm.Rules;
using System.Data;
using System.Dynamic;

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

        public const string SOPRANO = "SOPRANO";
        public const string ALTO = "ALTO";
        public const string TENORE = "TENORE";
        public const string BASS = "BASS";

        public const string A = "A";
        public const string B = "B";
        public const string C = "C";
        public const string D = "D";
        public const string E = "E";
        public const string F = "F";
        public const string G = "G";

        public static readonly List<string> SharpsQueue = ["F", "C", "G", "D", "A", "E", "B"];
        public static readonly List<string> FlatsQueue = ["B", "E", "A", "D", "G", "C", "F"];
        public static readonly List<string> AllNotes = ["C", "D", "E", "F", "G", "A", "B"];

        public static readonly VoiceDistance VoiceDistance = new();

        public static readonly Dictionary<(float, string), (string, int)> NoteMappings = new Dictionary<(float, string), (string, int)>()
        {
            { (6.5f, Constants.SOPRANO), (Constants.G, 3) },
            { (6.0f, Constants.SOPRANO), (Constants.A, 3) },
            { (5.5f, Constants.SOPRANO), (Constants.B, 3) },
            { (5.0f, Constants.SOPRANO), (Constants.C, 4) },
            { (4.5f, Constants.SOPRANO), (Constants.D, 4) },
            { (4.0f, Constants.SOPRANO), (Constants.E, 4) },
            { (3.5f, Constants.SOPRANO), (Constants.F, 4) },
            { (3.0f, Constants.SOPRANO), (Constants.G, 4) },
            { (2.5f, Constants.SOPRANO), (Constants.A, 4) },
            { (2.0f, Constants.SOPRANO), (Constants.B, 4) },
            { (1.5f, Constants.SOPRANO), (Constants.C, 5) },
            { (1.0f, Constants.SOPRANO), (Constants.D, 5) },
            { (0.5f, Constants.SOPRANO), (Constants.E, 5) },
            { (0.0f, Constants.SOPRANO), (Constants.F, 5) },
            { (-0.5f, Constants.SOPRANO), (Constants.G, 5) },
            { (-1.0f, Constants.SOPRANO), (Constants.A, 5) },
            { (-1.5f, Constants.SOPRANO), (Constants.B, 5) },
            { (-2.0f, Constants.SOPRANO), (Constants.C, 6) },

            { (6.5f, Constants.ALTO), (Constants.G, 3) },
            { (6.0f, Constants.ALTO), (Constants.A, 3) },
            { (5.5f, Constants.ALTO), (Constants.B, 3) },
            { (5.0f, Constants.ALTO), (Constants.C, 4) },
            { (4.5f, Constants.ALTO), (Constants.D, 4) },
            { (4.0f, Constants.ALTO), (Constants.E, 4) },
            { (3.5f, Constants.ALTO), (Constants.F, 4) },
            { (3.0f, Constants.ALTO), (Constants.G, 4) },
            { (2.5f, Constants.ALTO), (Constants.A, 4) },
            { (2.0f, Constants.ALTO), (Constants.B, 4) },
            { (1.5f, Constants.ALTO), (Constants.C, 5) },
            { (1.0f, Constants.ALTO), (Constants.D, 5) },
            { (0.5f, Constants.ALTO), (Constants.E, 5) },
            { (0.0f, Constants.ALTO), (Constants.F, 5) },
            { (-0.5f, Constants.ALTO), (Constants.G, 5) },
            { (-1.0f, Constants.ALTO), (Constants.A, 5) },
            { (-1.5f, Constants.ALTO), (Constants.B, 5) },
            { (-2.0f, Constants.ALTO), (Constants.C, 6) },

            { (7.0f, Constants.TENORE), (Constants.C, 2) },
            { (6.5f, Constants.TENORE), (Constants.D, 2) },
            { (6.0f, Constants.TENORE), (Constants.E, 2) },
            { (5.5f, Constants.TENORE), (Constants.F, 2) },
            { (5.0f, Constants.TENORE), (Constants.G, 2) },
            { (4.5f, Constants.TENORE), (Constants.A, 2) },
            { (4.0f, Constants.TENORE), (Constants.B, 2) },
            { (3.5f, Constants.TENORE), (Constants.C, 3) },
            { (3.0f, Constants.TENORE), (Constants.D, 3) },
            { (2.5f, Constants.TENORE), (Constants.E, 3) },
            { (2.0f, Constants.TENORE), (Constants.F, 3) },
            { (1.5f, Constants.TENORE), (Constants.G, 3) },
            { (1.0f, Constants.TENORE), (Constants.A, 3) },
            { (0.0f, Constants.TENORE), (Constants.B, 3) },
            { (-0.5f, Constants.TENORE), (Constants.C, 4) },
            { (-1.0f, Constants.TENORE), (Constants.D, 4) },
            { (-1.5f, Constants.TENORE), (Constants.E, 4) },
            { (-2.0f, Constants.TENORE), (Constants.F, 4) },
            { (-2.5f, Constants.TENORE), (Constants.G, 4) },

            { (7.0f, Constants.BASS), (Constants.C, 2) },
            { (6.5f, Constants.BASS), (Constants.D, 2) },
            { (6.0f, Constants.BASS), (Constants.E, 2) },
            { (5.5f, Constants.BASS), (Constants.F, 2) },
            { (5.0f, Constants.BASS), (Constants.G, 2) },
            { (4.5f, Constants.BASS), (Constants.A, 2) },
            { (4.0f, Constants.BASS), (Constants.B, 2) },
            { (3.5f, Constants.BASS), (Constants.C, 3) },
            { (3.0f, Constants.BASS), (Constants.D, 3) },
            { (2.5f, Constants.BASS), (Constants.E, 3) },
            { (2.0f, Constants.BASS), (Constants.F, 3) },
            { (1.5f, Constants.BASS), (Constants.G, 3) },
            { (1.0f, Constants.BASS), (Constants.A, 3) },
            { (0.0f, Constants.BASS), (Constants.B, 3) },
            { (-0.5f, Constants.BASS), (Constants.C, 4) },
            { (-1.0f, Constants.BASS), (Constants.D, 4) },
            { (-1.5f, Constants.BASS), (Constants.E, 4) },
            { (-2.0f, Constants.BASS), (Constants.F, 4) },
            { (-2.5f, Constants.BASS), (Constants.G, 4) }
        };
    }
}
