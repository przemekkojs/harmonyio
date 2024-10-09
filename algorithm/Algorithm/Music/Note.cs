namespace Algorithm.Music
{
    public enum NoteDirection
    {
        UP,
        DOWN
    }

    public enum Staff
    {
        UPPER,
        LOWER
    }

    public enum Voice
    {
        SOPRANO,
        ALTO,
        TENORE,
        BASS
    }

    public enum Accidental
    {
        SHARP,
        FLAT,
        DOUBLE_SHARP,
        DOUBLE_FLAT,
        NEUTRAL,
        NONE
    }

    public class Note
    {
        public RhytmicValue RhytmicValue { get => rhytmicValue; }
        public string Name { get => name; }
        public int Octave { get => octave; }
        public Accidental Accidental { get => accidental; }
        public NoteDirection NoteDirection { get => direction; }
        public Staff Staff { get => staff; }
        public FunctionComponent FunctionComponent { get => functionComponent; } 
        
        private readonly string name;        
        private readonly FunctionComponent functionComponent;        

        private int octave;
        private Accidental accidental;
        private Staff staff;
        private NoteDirection direction;
        private Voice voice;
        private RhytmicValue rhytmicValue;

        /*
         FOR UI:
            duration
            dotted         
         */

        public Note(string name, int octave, FunctionComponent functionComponent, RhytmicValue rhytmicValue, Voice voice, Accidental accidental=Accidental.NONE, bool neutralized=false)
        {
            this.name = name;
            this.octave = octave;
            this.rhytmicValue = rhytmicValue;
            this.accidental = accidental;
            this.functionComponent = functionComponent;
            this.voice = voice;

            UpdateNoteInfo(voice);

            if (neutralized)
            {
                if (accidental != Accidental.NONE)
                    throw new ArgumentException("Cannot simultainously add two accidentals.");

                this.accidental = Accidental.NEUTRAL;
            } 
        }

        public void UpdateNoteInfo(Voice voice)
        {
            this.voice = voice;

            switch (voice)
            {
                case Voice.SOPRANO:
                    this.direction = NoteDirection.UP;
                    this.staff = Staff.UPPER;
                    break;

                case Voice.ALTO:
                    this.direction = NoteDirection.DOWN;
                    this.staff = Staff.UPPER;
                    break;

                case Voice.TENORE:
                    this.direction = NoteDirection.UP;
                    this.staff = Staff.LOWER;
                    break;

                default:
                    this.direction = NoteDirection.DOWN;
                    this.staff = Staff.LOWER;
                    break;
            }
        }

        public override string ToString() => $"{name}{accidental}{octave}";
    }
}
