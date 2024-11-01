﻿using Algorithm.New.Utils;
using System.Text.Json.Serialization;

namespace Algorithm.New.Music
{
    public record Index
    {
        public int Bar { get; set; }
        public int Beat { get; set; }
        public int Duration { get; set; }
    }

    public class Stack
    {
        public Note Soprano { get; set; }
        public Note Alto { get; set; }
        public Note Tenore { get; set; }
        public Note Bass { get; set; }

        public Index Index { get; set; }

        public Stack(Index index, Note soprano, Note alto, Note tenore, Note bass)
        { 
            Soprano = soprano;
            Alto = alto;
            Tenore = tenore;
            Bass = bass;
            Index = index;
        }

        public Stack (Index index, string soprano, string alto, string tenore, string bass)
        {
            Soprano = new Note(soprano, 4);
            Alto = new Note(alto, 4);
            Tenore = new Note(tenore, 3);
            Bass = new Note(bass, 3);
            Index = index;

            SetOctaves();
        }

        // TODO: Bind properties
        [JsonConstructor]
        public Stack(int bar, int beat, int duration, string soprano, string alto, string tenore, string bass) : 
            this(new Index() { Bar = bar, Beat = beat, Duration = duration }, soprano, alto, tenore, bass) { }

        public Stack(Index index, List<string> notes) : this(index, notes[0], notes[1], notes[2], notes[3]) { }
        public Stack(Index index,List<Note> notes) : this(index,notes[0], notes[1], notes[2], notes[3]) { }
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
