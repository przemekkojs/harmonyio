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
    }
}
