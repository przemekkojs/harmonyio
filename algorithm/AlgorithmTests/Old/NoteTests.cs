using Algorithm.Old.Music;

namespace AlgorithmTests.Old
{
    public class NoteTests
    {
        [Fact]
        public void SopranoNotesEqual()
        {
            var C4 = new Note("C", 4, FunctionComponent.Root, Voice.SOPRANO);
            var C4_Neutralized = new Note("C", 4, FunctionComponent.Root, Voice.SOPRANO, neutralized: true);

            Assert.True(C4.Equals(C4_Neutralized), "These notes should be equal.");
        }

        [Fact]
        public void DifferentVoiceNotesEqual()
        {
            var C4 = new Note("C", 4, FunctionComponent.Root, Voice.SOPRANO);
            var C4_Neutralized = new Note("C", 4, FunctionComponent.Root, Voice.ALTO, neutralized: true);

            Assert.True(C4.Equals(C4_Neutralized), "These notes should be equal.");
        }

        [Fact]
        public void DifferentAccidentalNotesCreationEqual()
        {
            var C_Sharp_4 = new Note("C#", 4, FunctionComponent.Root, Voice.SOPRANO);
            var C_Sharp_4_Accidental = new Note("C#", 4, FunctionComponent.Root, Voice.SOPRANO, Accidental.SHARP);

            Assert.True(C_Sharp_4.Equals(C_Sharp_4_Accidental), "These notes should be equal.");
        }

        [Fact]
        public void IsNameSuperiorToAccidental()
        {
            var C_Sharp_4_Good = new Note("C#", 4, FunctionComponent.Root, Voice.SOPRANO);
            var C_Sharp_4_Bad = new Note("C#", 4, FunctionComponent.Root, Voice.SOPRANO, Accidental.FLAT); //Invalid accidental

            Assert.True(C_Sharp_4_Good.Equals(C_Sharp_4_Bad), "These notes should be equal.");
        }

        [Fact]
        public void AddingNeutralTest()
        {
            var C4 = new Note("C", 4, FunctionComponent.Root, Voice.SOPRANO);
            var noNetural = C4.Accidental == Accidental.NONE;

            C4.AddNeutral();
            var neutral = C4.Accidental == Accidental.NEUTRAL;

            Assert.True(noNetural && neutral, "These should be both true.");
        }
    }
}
