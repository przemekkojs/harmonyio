using Algorithm.New.Utils;
using System.Text.Json.Serialization;

namespace Algorithm.New.Music
{
    public class Stack
    {
        public Note? Soprano { get; set; } = null;
        public Note? Alto { get; set; } = null;
        public Note? Tenore { get; set; } = null;
        public Note? Bass { get; set; } = null;

        public Index Index { get; set; }

        public List<Note?> Notes { get; private set; }

        public Stack(Index index, Note? soprano, Note? alto, Note? tenore, Note? bass)
        { 
            Soprano = soprano;
            Alto = alto;
            Tenore = tenore;
            Bass = bass;
            Index = index;

            Notes = [Soprano, Alto, Tenore, Bass];        
        }

        public Stack (Index index, string soprano, string alto, string tenore, string bass)
        {
            if (soprano != string.Empty)
                Soprano = new Note(soprano, 4);
            else
                Soprano = null;

            if (alto != string.Empty)
                Alto = new Note(alto, 4);
            else
                Alto = null;

            if (tenore != string.Empty)
                Tenore = new Note(tenore, 3);
            else
                Tenore = null;

            if (bass != string.Empty)
                Bass = new Note(bass, 3);
            else
                Bass = null;


            Index = index;
            Notes = [Soprano, Alto, Tenore, Bass];

            SetOctaves();
        }

        [JsonConstructor]
        public Stack(int bar, int beat, int duration, string soprano, string alto, string tenore, string bass) : 
            this(new Index() { Bar = bar, Position = beat, Duration = duration }, soprano, alto, tenore, bass) { }

        public Stack(Index index, List<string> noteNames) : this(index, noteNames[0], noteNames[1], noteNames[2], noteNames[3]) { }
        public Stack(Index index, List<Note?> notes) : this(index, notes[0], notes[1], notes[2], notes[3]) { }
        public Stack (Index index, List<Component> components, Function function)
        {
            Soprano = new Note(Converters.ComponentToNote(components[0], function), 4);
            Alto = new Note(Converters.ComponentToNote(components[1], function), 4);
            Tenore = new Note(Converters.ComponentToNote(components[2], function), 3);
            Bass = new Note(Converters.ComponentToNote(components[3], function), 3);

            Index = index;
            Notes = [Soprano, Alto, Tenore, Bass];

            SetOctaves();
        }

        public void SetOctaves()
        {
            while (Tenore != null && Interval.IsLower(Tenore, Bass))
                Tenore.Octave++;

            while (Alto != null && Interval.IsLower(Alto, Tenore))
                Alto.Octave++;

            while (Soprano != null && Interval.IsLower(Soprano, Alto))
                Soprano.Octave++;
        }

        public void SetOctaves(Stack previous)
        {
            Interval.SetCloser(Bass, previous.Bass);
            SetOctaves();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Stack casted)
            {
                if (casted.Notes.Count != Notes.Count)
                    return false;

                if (!casted.Index.Equals(Index))
                    return false;

                for (int index = 0; index < casted.Notes.Count; index++)
                {
                    var note1 = Notes[index];
                    var note2 = casted.Notes[index];

                    if (note1 != null)
                    {
                        var notesEqual = note1.Equals(note2);

                        if (!notesEqual)
                            return false;
                    }
                }
            }
            else
                return false;

            return true;
        }

        public override string ToString() => $"{Soprano} {Alto} {Tenore} {Bass}";
    }
}
