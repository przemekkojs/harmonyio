namespace Algorithm.Music
{
    public class Note
    {
        public RhytmicValue RhytmicValue { get => rhytmicValue; }
        public string Name { get => name; }
        public string Octave { get => octave; }

        private readonly RhytmicValue rhytmicValue;
        private readonly string name;
        private readonly string octave;
        
        public Note(string name, string octave, RhytmicValue rhytmicValue)
        {
            this.name = name;
            this.octave = octave;
            this.rhytmicValue = rhytmicValue;
        }
    }
}
