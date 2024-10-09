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
            bool result = TestTemplate(
                tonation: new Tonation("C", Mode.MAJOR, 0, 0),
                symbol: new Symbol(false, FunctionSymbol.T),
                isMainFunction: true,
                expectedNotes: ["C", "E", "G"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void TonicInFSharpMinorTest()
        {
            bool result = TestTemplate(
                tonation: new Tonation("F#", Mode.MINOR, 0, 3),
                symbol: new Symbol(false, FunctionSymbol.T),
                isMainFunction: true,
                expectedNotes: ["F#", "A", "C#"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void Tonic7InGMinor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("G", Mode.MINOR, 2, 0),
                symbol: new Symbol(false, FunctionSymbol.T, added: [FunctionComponent.Seventh]),
                isMainFunction: true,
                expectedNotes: ["G", "Bb", "D", "F"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void Tonic9InBMajor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("B", Mode.MAJOR, 0, 5),
                symbol: new Symbol(false, FunctionSymbol.T, FunctionComponent.Root, added: [FunctionComponent.Ninth]),
                isMainFunction: true,
                expectedNotes: ["B", "D#", "F#", "A#", "C#"]
            );

            Assert.True(result, "This should be true");
        }

        private bool TestTemplate(Tonation tonation, Symbol symbol, bool isMainFunction, List<string> expectedNotes)
        {
            var TonicFunction = new Function(symbol, true, 1, 0);
            var Tonic = new Chord(TonicFunction, tonation);
            List<string> actualUniqueNoteNames = Tonic.UniqueNoteNames();

            expectedNotes.Sort();

            if (actualUniqueNoteNames.Count != expectedNotes.Count)
            {
                return false;
            }

            int index = 0;

            while (index < actualUniqueNoteNames.Count && actualUniqueNoteNames[index].Equals(expectedNotes[index]))
                index++;

            return true;
        }
    }
}
