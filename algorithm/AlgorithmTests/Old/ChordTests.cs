﻿using Algorithm.Old.Music;

namespace AlgorithmTests.Old
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
        public void mTInFSharpMinorTest()
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
        public void mT7InGMinor()
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
        public void TInBMajor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("B", Mode.MAJOR, 0, 5),
                symbol: new Symbol(false, FunctionSymbol.T, FunctionComponent.Root, added: [FunctionComponent.Ninth]),
                isMainFunction: true,
                expectedNotes: ["B", "D#", "F#", "A#", "C#"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void Sii7InCMajor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("C", Mode.MAJOR, 0, 0),
                symbol: new Symbol(false, FunctionSymbol.Sii, added: [FunctionComponent.Seventh]),
                isMainFunction: true,
                expectedNotes: ["D", "F", "A", "C"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void Sii9InCMajor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("C", Mode.MAJOR, 0, 0),
                symbol: new Symbol(false, FunctionSymbol.Sii, added: [FunctionComponent.Ninth]),
                isMainFunction: true,
                expectedNotes: ["D", "F", "A", "C", "E"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void mSii9InFMinor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("F", Mode.MINOR, 4, 0),
                symbol: new Symbol(false, FunctionSymbol.Sii, added: [FunctionComponent.Ninth]),
                isMainFunction: true,
                expectedNotes: ["G", "Bb", "Db", "F", "Ab"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void D67InCMajor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("C", Mode.MAJOR, 0, 0),
                symbol: new Symbol(false, FunctionSymbol.D, added: [FunctionComponent.Sixth]),
                isMainFunction: true,
                expectedNotes: ["G", "B", "E", "F"]
            );

            Assert.True(result, "This should be true");
        }

        [Fact]
        public void D67InEflatMajor()
        {
            bool result = TestTemplate(
                tonation: new Tonation("Eb", Mode.MAJOR, 3, 0),
                symbol: new Symbol(false, FunctionSymbol.D, added: [FunctionComponent.Sixth]),
                isMainFunction: true,
                expectedNotes: ["Bb", "D", "G", "Ab"]
            );

            Assert.True(result, "This should be true");
        }

        private static bool TestTemplate(Tonation tonation, Symbol symbol, bool isMainFunction, List<string> expectedNotes)
        {
            var TonicFunction = new Function(symbol, isMainFunction);
            var Tonic = new Chord(TonicFunction, tonation);
            List<string> actualUniqueNoteNames = Tonic.UniqueNoteNames();

            expectedNotes.Sort();

            if (actualUniqueNoteNames.Count != expectedNotes.Count)
                return false;

            int index = 0;

            while (index < actualUniqueNoteNames.Count && actualUniqueNoteNames[index].Equals(expectedNotes[index]))
                index++;

            return index == actualUniqueNoteNames.Count;
        }
    }
}
