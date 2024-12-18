﻿using Algorithm.New.Music;

namespace Algorithm.New.Utils
{
    public enum IntervalName
    {
        Unisono, AugmentedUnisono,
        DiminishedSecond, MinorSecond, MajorSecond, AugmentedSecond,
        DiminishedThird, MinorThird, MajorThird, AugmentedThird, DoubleAugmentedThird,
        DoubleDiminishedFourth, DiminishedFourth, PerfectFourth, AugmentedFourth, DoubleAugmentedFourth,
        Tritone,
        DoubleDiminishedFifth, DiminishedFifth, PerfectFifth, AugmentedFifth, DoubleAugmentedFifth,
        DoubleDiminishedSixth, DiminishedSixth, MinorSixth, MajorSixth, AugmentedSixth, DoubleAugmentedSixth,
        DoubleDiminishedSeventh, DiminishedSeventh, MinorSeventh, MajorSeventh, AugmentedSeventh,
        DiminishedOctave,
        WRONG
    }

    public static class Interval
    {
        private const int OCTAVE_SEMITONES = 12;

        public static readonly Dictionary<string, int> NoteSemitones = new()
        {
            { "C", 0 }, { "Dbb", 0 }, { "B#", 0 },
            { "C#", 1 }, { "Db", 1 }, { "Bx", 1 },
            { "D", 2 }, { "Cx", 2 }, { "Ebb", 2 },
            { "D#", 3 }, { "Eb", 3 }, { "Fbb", 3 },
            { "E", 4 }, { "Dx", 4 }, { "Fb", 4 },
            { "F", 5 }, { "E#", 5 }, { "Gbb", 5 },
            { "F#", 6 }, { "Gb", 6 }, { "Ex", 6 },
            { "G", 7 }, { "Fx", 7 }, { "Abb", 7 },
            { "G#", 8 }, { "Ab", 8 },
            { "A", 9 }, { "Gx", 9 }, { "Bbb", 9 },
            { "A#", 10 }, { "Bb", 10 }, { "Cbb", 10 },
            { "B", 11 }, { "Ax", 11 }, { "Cb", 11 }
        };

        /* INTERVAL MAPPINGS
         * 0:   1   2>>
         * 1:   1<  2>
         * 2:   2   3>>
         * 3:   2<  3>
         * 4:   3   4>
         * 5:   3<  4   5>>
         * 6:   3<< 4<  5>
         * 7:   5   6>>
         * 8:   5<  6>
         * 9:   6   7>
         * 10:  6<  7
         * 11:  7<  8>
         */

        // To działa tak:
        // Klucz główny daje nam listę wszystkich możliwych interwałów o danej liczbie półtonów
        // Klucze w liście dają nam informację, który interwał wybrać, w zależności jaki biały interwał wyjdzie
        // Czyli mamy <semitonesReal, [(semitonesWhite1, name1), (semitonesWhite2, name2), ...]
        // Niektóre są podwójnie, bo np. w przypadku tercji zmniejszonej (DiminishedThird), jako biały interwał
        // możemy dostać albo tercję wielką (np. dla tercji zmniejszonej <C#, Eb> mamy biały interwał <C, E> = tercja wielka (MajorThird),
        // a dla tercji zmniejszonej <B, Db> mamy biały interwał <B, D> = tercja mała (MinorThird)
        private static readonly Dictionary<int, List<(int, IntervalName)>> _intervalNames = new()
        {
            {  0, [(0, IntervalName.Unisono), (2, IntervalName.DiminishedSecond)] },
            {  1, [(0, IntervalName.AugmentedUnisono), (1, IntervalName.MinorSecond), (2, IntervalName.MinorSecond)] },
            {  2, [(1, IntervalName.MajorSecond), (2, IntervalName.MajorSecond), (3, IntervalName.DiminishedThird), (4, IntervalName.DiminishedThird)] },
            {  3, [(2, IntervalName.AugmentedSecond), (3, IntervalName.MinorThird), (4, IntervalName.MinorThird), (5, IntervalName.DoubleDiminishedFourth)] },
            {  4, [(3, IntervalName.MajorThird), (4, IntervalName.MajorThird), (5, IntervalName.DiminishedFourth)] },
            {  5, [(4, IntervalName.AugmentedThird), (5, IntervalName.PerfectFourth), (6, IntervalName.DiminishedFifth), (7, IntervalName.DoubleDiminishedFifth)] },
            {  6, [(3, IntervalName.DoubleAugmentedThird), (4, IntervalName.DoubleAugmentedThird), (5, IntervalName.AugmentedFourth), (6, IntervalName.DoubleDiminishedFifth), (7, IntervalName.DiminishedFifth)] },
            {  7, [(5, IntervalName.DoubleAugmentedFourth), (6, IntervalName.DiminishedFifth), (7, IntervalName.PerfectFifth), (8, IntervalName.DiminishedSixth), (9, IntervalName.DoubleDiminishedSixth)] },
            {  8, [(7, IntervalName.AugmentedFifth), (8, IntervalName.MinorSixth), (9, IntervalName.MinorSixth), (10, IntervalName.DoubleDiminishedSeventh)] },
            {  9, [(7, IntervalName.DoubleAugmentedFifth), (8, IntervalName.MajorSixth), (9, IntervalName.MajorSixth), (10, IntervalName.DiminishedSeventh)] },
            { 10, [(8, IntervalName.AugmentedSixth), (9, IntervalName.AugmentedSixth), (10, IntervalName.MinorSeventh), (11, IntervalName.MinorSeventh)] },
            { 11, [(9, IntervalName.DoubleAugmentedSixth), (10, IntervalName.MajorSeventh), (11, IntervalName.MajorSeventh)] }
        };

        private static readonly Dictionary<int, IntervalName> _basicIntervals = new()
        {
            { 0, IntervalName.Unisono },
            { 1, IntervalName.MinorSecond },
            { 2, IntervalName.MajorSecond },
            { 3, IntervalName.MinorThird },
            { 4, IntervalName.MajorThird },
            { 5, IntervalName.PerfectFourth },
            { 6, IntervalName.Tritone },
            { 7, IntervalName.PerfectFifth },
            { 8, IntervalName.MinorSixth },
            { 9, IntervalName.MajorSixth },
            { 10, IntervalName.MinorSeventh },
            { 11, IntervalName.MajorSeventh }
        };

        public static int Determinant(Note? note)
        {
            if (note == null)
                return 0;

            var octavePart = note.Octave * 12;
            var noteName = note.Name;

            if (note.Accidental == "bq")
                noteName = noteName[0].ToString();

            var namePart = NoteSemitones[noteName];

            return octavePart + namePart;
        }

        public static bool IsLower(Note? note, Note? target) => Determinant(note) < Determinant(target);

        public static void SetCloser(Note? note, Note? target)
        {
            if (note == null || target == null)
                return;

            note.Octave = target.Octave;

            var option1 = new Note(note.Name, note.Octave + 1);
            var option2 = new Note(note.Name, note.Octave - 1);

            var targetDeterminant = Determinant(target);

            var originalDiff = Math.Abs(Determinant(note) - targetDeterminant);
            var option1Diff = Math.Abs(Determinant(option1) - targetDeterminant);
            var option2Diff = Math.Abs(Determinant(option2) - targetDeterminant);

            var min = Math.Min(Math.Min(originalDiff, option1Diff), option2Diff);

            if (min == originalDiff)
                return;
            else if (min == option1Diff)
                note.Octave = option1.Octave;
            else
                note.Octave = option2.Octave;
        }

        public static int SemitonesBetween(Note? note1, Note? note2)
        {
            var det1 = Determinant(note1);
            var det2 = Determinant(note2);

            return Math.Abs(det1 - det2);
        }

        public static IntervalName IntervalBetween(Note? note1, Note? note2)
        {
            if (note1 == null || note2 == null)
                return IntervalName.WRONG;

            // Obsługa przypadku F-H, H-F
            if (note1.Name.Equals("B") && note2.Name.Equals("F"))
                return IntervalName.DiminishedFifth;
            else if (note1.Name.Equals("F") && note2.Name.Equals("B"))
                return IntervalName.AugmentedFourth;

            var whiteName1 = note1.Name[0].ToString();
            var whiteName2 = note2.Name[0].ToString();
            var octave1 = note1.Octave;
            var octave2 = note2.Octave;

            var tmp1 = new Note(whiteName1, octave1);
            var tmp2 = new Note(whiteName2, octave2);

            var semitonesReal = SemitonesBetween(note1, note2) % OCTAVE_SEMITONES;
            var semitonesWhite = SemitonesBetween(tmp1, tmp2) % OCTAVE_SEMITONES;

            var possibleReal = _intervalNames[semitonesReal];

            // To się nie powinno nigdy wydarzyć, ale na potrzeby testowej implementacji można zostawić
            if (possibleReal.Count == 0)
                return IntervalName.WRONG;

            var containsSemitone = possibleReal
                .Select(x => x.Item1)
                .Contains(semitonesWhite);

            if (!containsSemitone)
                return IntervalName.WRONG;

            var matching = possibleReal
                .Where(x => x.Item1 == semitonesWhite)
                .First();

            var matchingName = matching.Item2;

            return matchingName;
        }
    }
}
