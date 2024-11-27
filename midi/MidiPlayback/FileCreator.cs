using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.MusicTheory;

namespace MidiPlayback
{
    public class FileCreator
    {
        private const int SPACE_BETWEEN_CHORDS = 100;
        private const int CHORD_DURATION = 500;

        public static byte[] Create()
        {
            var midiFile = new MidiFile();

            var trackChunk = new TrackChunk();
            midiFile.Chunks.Add(trackChunk);

            long currentTime = 0;            

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

        private static void AddChord(TrackChunk trackChunk, NoteName[] notes, int octave, long duration, long startTime)
        {
            foreach (var noteName in notes)
            {
                var noteNumber = (SevenBitNumber)(12 * (octave + 1) + (int)noteName); // Calculate MIDI note number
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
