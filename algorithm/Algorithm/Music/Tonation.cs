namespace Algorithm.Music
{
    public class Tonation
    {
        public int NumberOfFlats { get => numberOfFlats; }
        public int NumberOfSharps { get => numberOfSharps; }
        public string Name { get => name; }
        public Mode Mode { get => mode; }

        private readonly int numberOfFlats;
        private readonly int numberOfSharps;

        private readonly string name;
        private readonly Mode mode;

        private readonly Scale scale;

        public Tonation(string name, Mode mode)
        {
            this.name = name;
            this.mode = mode;
            this.scale = new Scale(name, mode);

            DeductNumberOfAccidentals();
        }

        private void DeductNumberOfAccidentals()
        {

        }
    }
}
