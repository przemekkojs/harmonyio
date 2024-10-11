using System.Linq.Expressions;

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
        public string Name { get; private set; }
        public int Octave { get; private set; }
        public Accidental Accidental { get; private set; }
        public NoteDirection Direction { get; private set; }
        public Staff Staff { get; private set; }
        public FunctionComponent FunctionComponent { get; private set; } 
        public Voice Voice { get; private set; }                

        /*
         FOR UI:
            duration
            dotted         
         */

        public Note(string name, int octave, FunctionComponent functionComponent, Voice voice, Accidental accidental=Accidental.NONE, bool neutralized=false)
        {
            Octave = octave;
            FunctionComponent = functionComponent;

            UpdateNoteInfo(voice);
            AddAccidentals(name);

            if (neutralized)
            {
                if (accidental != Accidental.NONE && accidental != Accidental.NEUTRAL)
                    throw new ArgumentException("Cannot simultainously add two accidentals.");

                this.Accidental = Accidental.NEUTRAL;
            }
        }

        public void AddAccidentals(string name)
        {
            if (name.Length == 1)
            {
                this.Accidental = Accidental.NONE;
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

                this.Accidental = toAdd;
            }
            else
            {
                throw new ArgumentException("Name should be at least one letter");
            }

            // If everything was okay
            this.Name = name;
        }

        public void AddNeutral()
        {
            if (this.Accidental != Accidental.NONE)
                this.Name = this.Name[0].ToString();

            this.Accidental = Accidental.NEUTRAL;
        }

        public void UpdateNoteInfo(Voice voice)
        {
            this.Voice = voice;

            switch (voice)
            {
                case Voice.SOPRANO:
                    this.Direction = NoteDirection.UP;
                    this.Staff = Staff.UPPER;
                    break;

                case Voice.ALTO:
                    this.Direction = NoteDirection.DOWN;
                    this.Staff = Staff.UPPER;
                    break;

                case Voice.TENORE:
                    this.Direction = NoteDirection.UP;
                    this.Staff = Staff.LOWER;
                    break;

                default:
                    this.Direction = NoteDirection.DOWN;
                    this.Staff = Staff.LOWER;
                    break;
            }
        }

        public override string ToString() => $"{Name}{Octave}";

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            var casted = obj as Note;

            if (casted != null)
            {
                var checkName = (this.Name == casted.Name);
                var checkOctave = (this.Octave == casted.Octave);

                return (checkName && checkOctave);
            }

            return false;
        }
    }
}
