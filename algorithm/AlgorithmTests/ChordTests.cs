using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithm.Music;

namespace AlgorithmTests
{
    public class ChordTests
    {
        [Fact]
        public void TonicInCMajorTest()
        {
            var CMajor = new Tonation("C", Mode.MAJOR, 0, 0);
            var TonicSymbol = new Symbol(false, FunctionSymbol.T);
            var TonicFunction = new Function(TonicSymbol, 1, 0);
            var Tonic = new Chord(TonicFunction, CMajor);

            List<string> expectedUniqueNoteNames = new List<string>() { "C", "E", "G" };
            expectedUniqueNoteNames.Sort(); //For exact same behaviour

            List<string> actualUniqueNoteNames = Tonic.UniqueNoteNames();

            if (actualUniqueNoteNames.Count != expectedUniqueNoteNames.Count)
            {
                Assert.Fail();
                return;
            }

            int index = 0;

            while (index < actualUniqueNoteNames.Count && actualUniqueNoteNames[index].Equals(expectedUniqueNoteNames[index]))
            {
                index++;
            }
                

            Assert.True(index == actualUniqueNoteNames.Count, "This should be true.");
        }

        [Fact]
        public void TonicInFSharpMinorTest()
        {
            var CMajor = new Tonation("F#", Mode.MINOR, 0, 3);
            var TonicSymbol = new Symbol(true, FunctionSymbol.T);
            var TonicFunction = new Function(TonicSymbol, 1, 0);
            var Tonic = new Chord(TonicFunction, CMajor);

            List<string> expectedUniqueNoteNames = new List<string>() { "F#", "A", "C#" };
            expectedUniqueNoteNames.Sort(); //For exact same behaviour

            List<string> actualUniqueNoteNames = Tonic.UniqueNoteNames();

            if (actualUniqueNoteNames.Count != expectedUniqueNoteNames.Count)
            {
                Assert.Fail();
                return;
            }

            int index = 0;

            while (index < actualUniqueNoteNames.Count && actualUniqueNoteNames[index].Equals(expectedUniqueNoteNames[index]))
            {
                index++;
            }


            Assert.True(index == actualUniqueNoteNames.Count, "This should be true.");
        }

        [Fact]
        public void Tonic7InGMinor()
        {
            var CMajor = new Tonation("G", Mode.MINOR, 2, 0);
            var TonicSymbol = new Symbol(true, FunctionSymbol.T, added: [FunctionComponent.Seventh]);
            var TonicFunction = new Function(TonicSymbol, 1, 0);
            var Tonic = new Chord(TonicFunction, CMajor);

            List<string> expectedUniqueNoteNames = new List<string>() { "G", "Bb", "D", "F" };
            expectedUniqueNoteNames.Sort(); //For exact same behaviour

            List<string> actualUniqueNoteNames = Tonic.UniqueNoteNames();

            if (actualUniqueNoteNames.Count != expectedUniqueNoteNames.Count)
            {
                Assert.Fail();
                return;
            }

            int index = 0;

            while (index < actualUniqueNoteNames.Count && actualUniqueNoteNames[index].Equals(expectedUniqueNoteNames[index]))
            {
                index++;
            }


            Assert.True(index == actualUniqueNoteNames.Count, "This should be true.");
        }
    }
}
