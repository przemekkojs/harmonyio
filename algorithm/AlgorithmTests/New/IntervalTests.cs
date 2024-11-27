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
    }
}
