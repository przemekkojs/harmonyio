using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Algorithm.Old.Music
{
    internal class Interval
    {
        public Note Note1 { get => note1; }
        public Note Note2 { get => note2; }
        public string Name { get => name; }

        private readonly Note note1;
        private readonly Note note2;
        private readonly string name;

        private readonly int semitones;

        public Interval(Note note1, Note note2)
        {
            this.note1 = note1;
            this.note2 = note2;

            DeductName();
            DeductSemitones();
        }

        private void DeductName()
        {

        }

        private void DeductSemitones()
        {

        }

        public static List<Note> NotesAbove(Note? note, int semitones)
        {
            if (note == null)
                throw new ArgumentException("Note cannot be null");

            if (semitones == 0)
                return [note];

            return null;
        }

        public static List<string> NoteNamesAbove(string name, int semitones)
        {
            return null;
        }

        public static int SemitonesBetween(Note? note1, Note? note2)
        {
            if (note1 == null || note2 == null)
                return 0;

            return 0;
        }

        public static void SetClosestTo(Note toSet, Note target)
        {
            var toCheck1 = new Note(toSet);
            var toCheck2 = new Note(toSet);

            toCheck1.Octave += 1;
            toCheck2.Octave -= 1;

            var semitonesReal = SemitonesBetween(toSet, target);
            var semitonesToCheck1 = SemitonesBetween(toCheck1, target);
            var semitonesToCheck2 = SemitonesBetween(toCheck2, target);

            var minimum = Math.Min(Math.Min(semitonesReal, semitonesToCheck1), semitonesToCheck2);

            if (minimum == semitonesReal)
                return;
            else if (minimum == semitonesToCheck1)
                toSet.Octave = toCheck1.Octave;
            else if (minimum == semitonesToCheck2)
                toSet.Octave = toCheck2.Octave;
        }
    }
}
