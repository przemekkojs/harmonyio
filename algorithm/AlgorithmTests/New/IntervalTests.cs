using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace AlgorithmTests.New
{
    public class IntervalTests
    {
        [Fact]
        public void NotesNullTest()
        {
            Note? note1 = null;
            Note note2 = new ("C", 4);

            Assert.Equal(IntervalName.WRONG, Interval.IntervalBetween(note1, note2));
            Assert.Equal(IntervalName.WRONG, Interval.IntervalBetween(note1, note1));
            Assert.Equal(IntervalName.WRONG, Interval.IntervalBetween(note2, note1));
            Assert.NotEqual(IntervalName.WRONG, Interval.IntervalBetween(note2, note2));
        }

        [Fact]
        public void SameNotesTest()
        {
            Note note1 = new("C", 4);
            Note note2 = new("C", 4);

            Assert.Equal(IntervalName.Unisono, Interval.IntervalBetween(note1, note2));
            Assert.Equal(IntervalName.Unisono, Interval.IntervalBetween(note1, note1));
            Assert.Equal(IntervalName.Unisono, Interval.IntervalBetween(note2, note2));
            Assert.Equal(IntervalName.Unisono, Interval.IntervalBetween(note2, note1));
        }

        [Fact]
        public void SemitonesTest()
        {
            Note? origin = new("C", 4);

            List<Note> compared =
            [
                new("C#", 4),
                new("D", 4),
                new("D#", 4),
                new("E", 4),
                new("F", 4),
                new("F#", 4),
                new("G", 4),
                new("G#", 4),
                new("A", 4),
                new("A#", 4),
                new("B", 4),
                new("C", 5),
            ];

            for (int index = 0; index < compared.Count; index++)
            {
                var semitones = Interval.SemitonesBetween(origin, compared[index]);
                Assert.Equal(semitones, index + 1);
            }
        }

        [Fact]
        public void DifferentNotesTest ()
        {
            Note C4 = new("C", 4);
            Note Cs4 = new("C#", 4);
            Note D4 = new("D", 4);
            Note Ds4 = new("D#", 4);
            Note E4 = new("E", 4);
            Note F4 = new("F", 4);
            Note Fs4 = new("F#", 4);
            Note G4 = new("G", 4);
            Note Gs4 = new("G#", 4);
            Note A4 = new("A", 4);
            Note As4 = new("A#", 4);
            Note B4 = new("B", 4);
            Note C5 = new("C", 5);

            List<((Note, Note), IntervalName)> testPairs =
            [
                ((C4, C4), IntervalName.Unisono),
                ((C4, Cs4), IntervalName.AugmentedUnisono),
                ((C4, D4), IntervalName.MajorSecond),
                ((C4, Ds4), IntervalName.AugmentedSecond),
                ((C4, E4), IntervalName.MajorThird),
                ((C4, F4), IntervalName.PerfectFourth),
                ((C4, Fs4), IntervalName.AugmentedFourth),
                ((C4, G4), IntervalName.PerfectFifth),
                ((C4, Gs4), IntervalName.AugmentedFifth),
                ((C4, A4), IntervalName.MajorSixth),
                ((C4, As4), IntervalName.MinorSeventh),
                ((C4, B4), IntervalName.MajorSeventh),
                ((C4, C5), IntervalName.Unisono),
            ];

            foreach (var pair in testPairs)
            {
                var note1 = pair.Item1.Item1;
                var note2 = pair.Item1.Item2;
                var expected = pair.Item2;
                var intervalName = Interval.IntervalBetween(note1, note2);

                Assert.Equal(intervalName, expected);
            }
        }
    }
}
