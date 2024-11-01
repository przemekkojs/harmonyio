using System.Linq.Expressions;

namespace Algorithm.Old.Music
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
        public string Name { get; private set; }
        public int Octave { get; set; }
        public Accidental Accidental { get; set; }
        public NoteDirection Direction { get; set; }
        public Staff Staff { get; set; }
        public FunctionComponent FunctionComponent { get => functionComponent; set => SetFunctionComponent(value); }
        public Voice Voice { get; private set; }

        private FunctionComponent functionComponent;

        /*
         FOR UI:
            duration
            dotted         
         */

        public Note(string name, int octave, Voice voice, Accidental accidental = Accidental.NONE, bool neutralized = false)
        {
            Name = name;
            Octave = octave;

            UpdateNoteInfo(voice);
            AddAccidentals(name);

            if (neutralized)
            {
                if (accidental != Accidental.NONE && accidental != Accidental.NEUTRAL)
                    throw new ArgumentException("Cannot simultainously add two accidentals.");

                Accidental = Accidental.NEUTRAL;
            }

            Staff = voice switch
            {
                Voice.SOPRANO => Staff.UPPER,
                Voice.ALTO => Staff.UPPER,
                Voice.TENORE => Staff.LOWER,
                Voice.BASS => Staff.LOWER,
                _ => throw new ArgumentException("Invalid voice")
            };
        }

        public Note(string name, int octave, FunctionComponent functionComponent, Voice voice, Accidental accidental = Accidental.NONE, bool neutralized = false) :
            this(name, octave, voice, accidental, neutralized) => this.functionComponent = functionComponent;

        public Note(Note other) : this(other.Name, other.Octave, other.FunctionComponent, other.Voice, other.Accidental, other.Accidental == Accidental.NEUTRAL) { }

        private void SetFunctionComponent(FunctionComponent component)
        {
            functionComponent = component;
        }

        public void AddAccidentals(string name)
        {
            if (name.Length == 1)
            {
                Accidental = Accidental.NONE;
            }
            else if (name.Length > 1)
            {
                string accindentalString = name[1..];

                Accidental toAdd = accindentalString switch
                {
                    "#" => Accidental.SHARP, // Sharp
                    "b" => Accidental.FLAT, // Flat
                    "x" => Accidental.DOUBLE_SHARP, // Double sharp
                    "bb" => Accidental.DOUBLE_FLAT, // Double flat
                    "bq" => Accidental.NEUTRAL, // Neutral
                    _ => throw new ArgumentException("Cannot add non-existent accidental - only [#, b, x, bb, bq].")
                };

                if (toAdd == Accidental.NEUTRAL)
                    name = name[0].ToString();

                Accidental = toAdd;
            }
            else
            {
                throw new ArgumentException("Name should be at least one letter");
            }

            // If everything was okay
            Name = name;
        }

        public void AddNeutral()
        {
            if (Accidental != Accidental.NONE)
                Name = Name[0].ToString();

            Accidental = Accidental.NEUTRAL;
        }

        public void UpdateNoteInfo(Voice voice)
        {
            Voice = voice;

            switch (voice)
            {
                case Voice.SOPRANO:
                    Direction = NoteDirection.UP;
                    Staff = Staff.UPPER;
                    break;

                case Voice.ALTO:
                    Direction = NoteDirection.DOWN;
                    Staff = Staff.UPPER;
                    break;

                case Voice.TENORE:
                    Direction = NoteDirection.UP;
                    Staff = Staff.LOWER;
                    break;

                default:
                    Direction = NoteDirection.DOWN;
                    Staff = Staff.LOWER;
                    break;
            }
        }

        public override string ToString() => $"{Name}{Octave}";

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Note casted)
            {
                var checkName = Name == casted.Name;
                var checkOctave = Octave == casted.Octave;

                return checkName && checkOctave;
            }

            return false;
        }
    }
}
