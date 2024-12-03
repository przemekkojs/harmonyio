using Algorithm.New.Algorithm;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

namespace MidiPlayback
{
    public class FileCreator
    {
        private const int SPACE_BETWEEN_CHORDS = 100;
        private const int CHORD_DURATION = 500;

        public static byte[] Create(Solution solution)
        {
            var midiFile = new MidiFile();
            var trackChunk = new TrackChunk();
            long currentTime = 0;

            midiFile.Chunks.Add(trackChunk);

            var cMajor = new[] { NoteName.C, NoteName.E, NoteName.G };
            var fMajor = new[] { NoteName.F, NoteName.A, NoteName.C };

            AddChord(trackChunk, cMajor, 4, CHORD_DURATION, currentTime);
            currentTime = SPACE_BETWEEN_CHORDS;
            AddChord(trackChunk, fMajor, 4, CHORD_DURATION, currentTime);
            currentTime = SPACE_BETWEEN_CHORDS;
            AddChord(trackChunk,cMajor, 4, CHORD_DURATION, currentTime);

            byte[] midiData = GetMidiFileData(midiFile);
            return midiData;
        }

        private static List<(NoteName, int)> CreateChord(Algorithm.New.Music.Stack stack)
        {
            List<(NoteName, int)> result = [];
            var stackNotes = stack.Notes;

            foreach(var note in stackNotes)
            {
                if (note == null)
                    continue;

                var midiNote = NoteToMidiNote(note);
                result.Add(midiNote);
            }

            return result;
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

        private static void AddChord(
            TrackChunk trackChunk, NoteName[] notes, int octave, long duration, long startTime)
        {
            foreach (var noteName in notes)
            {
                var noteNumber = (SevenBitNumber)(12 * (octave + 1) + (int)noteName);
                trackChunk.Events.Add(new NoteOnEvent(noteNumber, (SevenBitNumber)100) { DeltaTime = startTime });
                trackChunk.Events.Add(new NoteOffEvent(noteNumber, (SevenBitNumber)0) { DeltaTime = duration });
                startTime = 0;
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
