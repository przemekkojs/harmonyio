namespace Algorithm.Music
{
    internal class Chord
    {
        public Function Function { get => function; }
        public Tonation Tonation { get => tonation; }
        public List<Note> Notes { get => notes; }

        private readonly Function function;
        private readonly Tonation tonation;
        private readonly List<Note> notes;

        public Chord(Function function, Tonation tonation, List<Note> notes)
        {
            this.function = function;
            this.tonation = tonation;
            this.notes = notes;
        }
    }
}
