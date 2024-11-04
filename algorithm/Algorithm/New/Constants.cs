namespace Algorithm.New
{
    public static class Constants
    {
        public const int NOTES_IN_FUNCTION = 4;
        public const int NOTES_IN_TONATION = 7;

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

        // public static readonly VoiceDistance VoiceDistance = new();

        public static readonly Dictionary<(float, string), (string, int)> NoteMappings = new()
        {
            { (6.5f, SOPRANO), (G, 3) },
            { (6.0f, SOPRANO), (A, 3) },
            { (5.5f, SOPRANO), (B, 3) },
            { (5.0f, SOPRANO), (C, 4) },
            { (4.5f, SOPRANO), (D, 4) },
            { (4.0f, SOPRANO), (E, 4) },
            { (3.5f, SOPRANO), (F, 4) },
            { (3.0f, SOPRANO), (G, 4) },
            { (2.5f, SOPRANO), (A, 4) },
            { (2.0f, SOPRANO), (B, 4) },
            { (1.5f, SOPRANO), (C, 5) },
            { (1.0f, SOPRANO), (D, 5) },
            { (0.5f, SOPRANO), (E, 5) },
            { (0.0f, SOPRANO), (F, 5) },
            { (-0.5f, SOPRANO), (G, 5) },
            { (-1.0f, SOPRANO), (A, 5) },
            { (-1.5f, SOPRANO), (B, 5) },
            { (-2.0f, SOPRANO), (C, 6) },

            { (6.5f, ALTO), (G, 3) },
            { (6.0f, ALTO), (A, 3) },
            { (5.5f, ALTO), (B, 3) },
            { (5.0f, ALTO), (C, 4) },
            { (4.5f, ALTO), (D, 4) },
            { (4.0f, ALTO), (E, 4) },
            { (3.5f, ALTO), (F, 4) },
            { (3.0f, ALTO), (G, 4) },
            { (2.5f, ALTO), (A, 4) },
            { (2.0f, ALTO), (B, 4) },
            { (1.5f, ALTO), (C, 5) },
            { (1.0f, ALTO), (D, 5) },
            { (0.5f, ALTO), (E, 5) },
            { (0.0f, ALTO), (F, 5) },
            { (-0.5f, ALTO), (G, 5) },
            { (-1.0f, ALTO), (A, 5) },
            { (-1.5f, ALTO), (B, 5) },
            { (-2.0f, ALTO), (C, 6) },

            { (6.0f, TENORE), (C, 2) },
            { (5.5f, TENORE), (D, 2) },
            { (5.0f, TENORE), (E, 2) },
            { (4.5f, TENORE), (F, 2) },
            { (4.0f, TENORE), (G, 2) },
            { (3.5f, TENORE), (A, 2) },
            { (3.0f, TENORE), (B, 2) },
            { (2.5f, TENORE), (C, 3) },
            { (2.0f, TENORE), (D, 3) },
            { (1.5f, TENORE), (E, 3) },
            { (1.0f, TENORE), (F, 3) },
            { (0.5f, TENORE), (G, 3) },
            { (0.0f, TENORE), (A, 3) },
            { (-0.5f, TENORE), (B, 3) },
            { (-1.0f, TENORE), (C, 3) },
            { (-1.5f, TENORE), (D, 4) },
            { (-2.0f, TENORE), (E, 4) },
            { (-2.5f, TENORE), (F, 4) },
            { (-3.0f, TENORE), (G, 4) },

            { (6.0f, BASS), (C, 2) },
            { (5.5f, BASS), (D, 2) },
            { (5.0f, BASS), (E, 2) },
            { (4.5f, BASS), (F, 2) },
            { (4.0f, BASS), (G, 2) },
            { (3.5f, BASS), (A, 2) },
            { (3.0f, BASS), (B, 2) },
            { (2.5f, BASS), (C, 3) },
            { (2.0f, BASS), (D, 3) },
            { (1.5f, BASS), (E, 3) },
            { (1.0f, BASS), (F, 3) },
            { (0.5f, BASS), (G, 3) },
            { (0.0f, BASS), (A, 3) },
            { (-0.5f, BASS), (B, 3) },
            { (-1.0f, BASS), (C, 3) },
            { (-1.5f, BASS), (D, 4) },
            { (-2.0f, BASS), (E, 4) },
            { (-2.5f, BASS), (F, 4) },
            { (-3.0f, BASS), (G, 4) },
        };

        public const string SOLUTION_STRING = @"
            {
            ""metreValue"": 4,
            ""metreCount"": 2,
            ""sharpsCount"": 0,
            ""flatsCount"": 0,
            ""jsonNotes"": [
                {
                    ""Line"": 1.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""SOPRANO"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 0
                },
                {
                    ""Line"": 4.0,
                    ""AccidentalName"": """",
                    ""Voice"": ""ALTO"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 0
                },
                {
                    ""Line"": 0.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""TENORE"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 0
                },
                {
                    ""Line"": 2.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""BASS"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 0
                },
                {
                    ""Line"": 1.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""SOPRANO"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 1
                },
                {
                    ""Line"": 3.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""ALTO"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 1
                },
                {
                    ""Line"": 0.0,
                    ""AccidentalName"": """",
                    ""Voice"": ""TENORE"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 1
                },
                {
                    ""Line"": 2.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""BASS"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 1
                },
                {
                    ""Line"": 2.0,
                    ""AccidentalName"": """",
                    ""Voice"": ""SOPRANO"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 2
                },
                {
                    ""Line"": 3.0,
                    ""AccidentalName"": """",
                    ""Voice"": ""ALTO"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 2
                },
                {
                    ""Line"": 0.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""TENORE"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 2
                },
                {
                    ""Line"": 2.0,
                    ""AccidentalName"": """",
                    ""Voice"": ""BASS"",
                    ""Value"": 4,
                    ""BarIndex"": 0,
                    ""VerticalIndex"": 2
                },
                {
                    ""Line"": 1.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""SOPRANO"",
                    ""Value"": 12,
                    ""BarIndex"": 1,
                    ""VerticalIndex"": 0
                },
                {
                    ""Line"": 4.0,
                    ""AccidentalName"": """",
                    ""Voice"": ""ALTO"",
                    ""Value"": 12,
                    ""BarIndex"": 1,
                    ""VerticalIndex"": 0
                },
                {
                    ""Line"": 0.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""TENORE"",
                    ""Value"": 12,
                    ""BarIndex"": 1,
                    ""VerticalIndex"": 0
                },
                {
                    ""Line"": 2.5,
                    ""AccidentalName"": """",
                    ""Voice"": ""BASS"",
                    ""Value"": 12,
                    ""BarIndex"": 1,
                    ""VerticalIndex"": 0
                }
            ]
        }";

        public const string PROBLEM_STRING = @"
            {
                ""question"": """",
                ""sharpsCount"": 0,
                ""flatsCount"": 0,
                ""metreValue"": ""4"",
                ""metreCount"": ""2"",
                ""task"": [
                    {
                        ""minor"": false,
                        ""symbol"": ""T"",
                        ""position"": """",
                        ""root"": """",
                        ""removed"": """",
                        ""alterations"": [],
                        ""added"": [],
                        ""barIndex"": ""0"",
                        ""verticalIndex"": ""0""
                    },
                    {
                        ""minor"": false,
                        ""symbol"": ""S"",
                        ""position"": """",
                        ""root"": """",
                        ""removed"": """",
                        ""alterations"": [],
                        ""added"": [],
                        ""barIndex"": ""0"",
                        ""verticalIndex"": ""1""
                    },
                    {
                        ""minor"": false,
                        ""symbol"": ""D"",
                        ""position"": """",
                        ""root"": """",
                        ""removed"": """",
                        ""alterations"": [],
                        ""added"": [],
                        ""barIndex"": ""0"",
                        ""verticalIndex"": ""2""
                    },
                    {
                        ""minor"": false,
                        ""symbol"": ""T"",
                        ""position"": """",
                        ""root"": """",
                        ""removed"": """",
                        ""alterations"": [],
                        ""added"": [],
                        ""barIndex"": ""0"",
                        ""verticalIndex"": ""3""
                    }
                ]
            }
        ";
    }
}
