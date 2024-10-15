using System.Diagnostics.CodeAnalysis;

namespace Algorithm.Music
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

        public static List<Note> NotesAbove (Note? note, int semitones)
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
    }
}
