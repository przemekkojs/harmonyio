using Algorithm.New.Music;

namespace Algorithm.New.Utils
{
    public static class Interval
    {
        public static Dictionary<string, int> NoteSemitones = new()
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

        public static int Determinant(Note? note)
        {
            if (note == null)
                return 0;

            var octavePart = note.Octave * 4;
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
    }
}
