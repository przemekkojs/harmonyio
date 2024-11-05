using System.Runtime.CompilerServices;

namespace Algorithm.Old.Music
{
    public class Scale
    {
        public Mode Mode { get => mode; }
        public string Name { get => name; }
        public List<string> NoteNames { get => noteNames; }
        public string this[int index] => noteNames[index];

        private readonly Mode mode;
        private readonly string name;
        private readonly List<string> noteNames;

        public Scale(string name, Mode mode)
        {
            this.mode = mode;
            this.name = name;

            noteNames = new List<string>();
        }
    }
}
