using Algorithm.New.Algorithm;
using Algorithm.New.Music;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.MusicTheory;

namespace MidiPlayback
{
    public class FileCreator
    {
        private const int SPACE_BETWEEN_CHORDS = 2;
        private const int SIXTEENTH_NOTE_DURATION = 10;

        public static byte[] Create(List<Stack> stacks)
        {
            var midiFile = new MidiFile();
            var trackChunk = new TrackChunk();
            long currentTime = 0;

            midiFile.Chunks.Add(trackChunk);

            foreach (var stack in stacks)
            {
                var chordInfo = CreateChord(stack);
                var chord = chordInfo.Item1;
                var duration = stack.Index.Duration * SIXTEENTH_NOTE_DURATION;

                AddChord(trackChunk, chord, duration, currentTime);
                currentTime = SPACE_BETWEEN_CHORDS; // Możliwe, że zwykłe = trzeba dać...
            }

            byte[] midiData = GetMidiFileData(midiFile);
            return midiData;
        }

        public static byte[] Create(Solution solution)
        {
            var stacks = solution.Stacks;
            return Create(stacks);
        }

        private static (List<(NoteName, int)>, int) CreateChord(Stack stack)
        {
            List<(NoteName, int)> result = [];
            var stackNotes = stack.Notes;
            var duration = stack.Index.Duration;

            foreach (var note in stackNotes)
            {
                if (note == null)
                    continue;

                var midiNote = NoteToMidiNote(note);
                result.Add(midiNote);
            }

            if (result.Count > 0)
            {
                while (result.Count < 4)
                {
                    result.Add(result[0]);
                }
            }            

            return (result, duration * SIXTEENTH_NOTE_DURATION);
        }

        private static (NoteName, int) NoteToMidiNote(Algorithm.New.Music.Note note)
        {
            var octave = note.Octave;
            var name = note.Name;

            var noteName = name switch
            {
                "B#" => NoteName.C,
                "C" => NoteName.C,
                "Dbb" => NoteName.C,
                "C#" => NoteName.CSharp,
                "Db" => NoteName.CSharp,
                "Cx" => NoteName.D,
                "D" => NoteName.D,
                "Ebb" => NoteName.D,
                "D#" => NoteName.DSharp,
                "Eb" => NoteName.DSharp,
                "Fbb" => NoteName.DSharp,
                "Dx" => NoteName.E,
                "Fb" => NoteName.E,
                "E" => NoteName.E,
                "E#" => NoteName.F,
                "F" => NoteName.F,
                "Gbb" => NoteName.F,
                "Ex" => NoteName.FSharp,
                "F#" => NoteName.FSharp,
                "Gb" => NoteName.FSharp,
                "Fx" => NoteName.G,
                "G" => NoteName.G,
                "Abb" => NoteName.G,
                "G#" => NoteName.GSharp,
                "Ab" => NoteName.GSharp,
                "A" => NoteName.A,
                "Gx" => NoteName.A,
                "Bbb" => NoteName.A,
                "A#" => NoteName.ASharp,
                "Bb" => NoteName.ASharp,
                "Ax" => NoteName.B,
                "Cb" => NoteName.B,
                "B" => NoteName.B,
                _ => NoteName.B
            };

            return (noteName, octave);
        }

        private static SevenBitNumber GetNoteNumber(NoteName noteName, int octave) =>
            (SevenBitNumber)(12 * (octave + 1) + (int)noteName);

        private static void AddChord(TrackChunk trackChunk, List<(NoteName, int)> notes, long duration, long startTime)
        {
            foreach (var note in notes)
            {
                var noteName = note.Item1;
                var octave = note.Item2;
                var noteNumber = GetNoteNumber(noteName, octave);

                trackChunk.Events.Add(new NoteOnEvent(noteNumber, (SevenBitNumber)100) { DeltaTime = startTime });

                startTime = 0;
            }

            foreach (var note in notes)
            {
                var noteName = note.Item1;
                var octave = note.Item2;
                var noteNumber = GetNoteNumber(noteName, octave);

                trackChunk.Events.Add(new NoteOffEvent(noteNumber, (SevenBitNumber)0) { DeltaTime = duration });
            }
        }

        private static byte[] GetMidiFileData(MidiFile midiFile)
        {
            using var memoryStream = new MemoryStream();
            midiFile.Write(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
