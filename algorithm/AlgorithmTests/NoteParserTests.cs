using Algorithm.Communication;
using Algorithm.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTests
{
    public class NoteParserTests
    {
        [Fact]
        public void ParsedTwiceEqualsOrigin()
        {
            Note origin = new(
                name: "C",
                octave: 4,
                voice: Voice.SOPRANO
            );

            string deserializedNote = NoteParser.ParseNoteToJson(
                note: origin,
                rhytmicValue: RhytmicValue.QUARTER_NOTE,
                bar: 1,
                stackInBar: 1
            );

            Note serializedNote = NoteParser.ParseJsonToNote(deserializedNote).Note;

            Assert.True(origin.Equals(serializedNote));
        }

        [Fact]
        public void ParseFromNoteToJsonTest()
        {
            string testString = "{\"Name\":\"C\",\"Duration\":4,\"Octave\":4,\"Bar\":1,\"Stack\":1,\"Staff\":1,\"Voice\":\"SOPRANO\",\"Neutralized\":false}";
            string parsedString = NoteParser.ParseNoteToJson(new Note(name: "C", octave: 4, voice: Voice.SOPRANO), RhytmicValue.QUARTER_NOTE, 1, 1);

            Assert.Equal(testString, parsedString);
        }

        [Fact]
        public void ParseFromJsonToNoteTest()
        {
            string testString = "{\"Name\":\"C\",\"Duration\":4,\"Octave\":4,\"Bar\":1,\"Stack\":1,\"Staff\":1,\"Voice\":\"SOPRANO\",\"Neutralized\":false}";
            Note testNote = new Note("C", 4, Voice.SOPRANO);
            Note parsedNote = NoteParser.ParseJsonToNote(testString).Note;

            Assert.Equal(testNote, parsedNote);
        }
    }
}
