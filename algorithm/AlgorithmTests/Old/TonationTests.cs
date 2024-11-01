using Algorithm.Old.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTests.Old
{
    public class TonationTests
    {
        [Fact]
        public void CMajorNotesTest()
        {
            List<string> good = ["C", "D", "E", "F", "G", "A", "B"];
            var tonation = new Tonation("C", Mode.MAJOR, 0, 0);
            bool equal = true;

            for (int index = 0; index < good.Count; index++)
            {
                if (good[index] != tonation[index])
                {
                    equal = false;
                }
            }

            Assert.True(equal, "These should be true.");
        }

        [Fact]
        public void CMinorNotesTest()
        {
            List<string> good = ["C", "D", "Eb", "F", "G", "Ab", "Bb"];
            var tonation = new Tonation("C", Mode.MINOR, 3, 0);
            bool equal = true;

            for (int index = 0; index < good.Count; index++)
            {
                if (good[index] != tonation[index])
                {
                    equal = false;
                }
            }

            Assert.True(equal, "These should be true.");
        }

        [Fact]
        public void FSharpMajorNotesTest()
        {
            List<string> good = ["F#", "G#", "A#", "B", "C#", "D#", "E#"];
            var tonation = new Tonation("F#", Mode.MAJOR, 0, 6);
            bool equal = true;

            for (int index = 0; index < good.Count; index++)
            {
                if (good[index] != tonation[index])
                {
                    equal = false;
                }
            }

            Assert.True(equal, "These should be true.");
        }
    }
}
