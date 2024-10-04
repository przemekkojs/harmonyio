namespace Algorithm.Music
{
    public class Scale
    {
        private readonly Mode mode;
        private readonly string name;

        private readonly List<Note> notes;

        public Scale(string name, Mode mode)
        {
            this.mode = mode;
            this.name = name;

            DeductNotes();
        }

        private void DeductNotes()
        {

        }
    }
}
