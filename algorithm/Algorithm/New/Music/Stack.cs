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

            if (alto != string.Empty)
                Alto = new Note(alto, 4);

            if (tenore != string.Empty)
                Tenore = new Note(tenore, 3);

            if (bass != string.Empty)
                Bass = new Note(bass, 3);


            Index = index;

            SetOctaves();
        }

        // TODO: Bind properties
        [JsonConstructor]
        public Stack(int bar, int beat, int duration, string soprano, string alto, string tenore, string bass) : 
            this(new Index() { Bar = bar, Beat = beat, Duration = duration }, soprano, alto, tenore, bass) { }

        public Stack(Index index, List<string> notes) : this(index, notes[0], notes[1], notes[2], notes[3]) { }
        public Stack(Index index, List<Note?> notes) : this(index, notes[0], notes[1], notes[2], notes[3]) { }
        public Stack (Index index, List<Component> components, Function function, Tonation tonation)
        {
            Index = index;

            Soprano = new Note(Converters.ComponentToNote(components[0], function), 4);
            Alto = new Note(Converters.ComponentToNote(components[1], function), 4);
            Tenore = new Note(Converters.ComponentToNote(components[2], function), 3);
            Bass = new Note(Converters.ComponentToNote(components[3], function), 3);

            SetOctaves();
        }

        public void SetOctaves()
        {
            while (Interval.IsLower(Tenore, Bass))
                Tenore.Octave++;

            while (Interval.IsLower(Alto, Tenore))
                Tenore.Octave++;

            while (Interval.IsLower(Soprano, Alto))
                Tenore.Octave++;
        }

        public void SetOctaves(Stack previous)
        {
            Interval.SetCloser(Bass, previous.Bass);
            SetOctaves();
        }
    }
}
