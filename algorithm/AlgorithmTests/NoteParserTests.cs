using Algorithm.Communication;
using Algorithm.Music;

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
            string testString = "{\"Line\":5.0,\"AccidentalName\":\"\",\"Voice\":\"SOPRANO\",\"Value\":4,\"BarIndex\":1,\"VerticalIndex\":1}";
            string parsedString = NoteParser.ParseNoteToJson(new Note(name: "C", octave: 4, voice: Voice.SOPRANO), RhytmicValue.QUARTER_NOTE, 1, 1);

            Assert.Equal(testString, parsedString);
        }

        [Fact]
        public void ParseFromJsonToNoteTest()
        {            
            Note testNote = new Note("C", 4, Voice.SOPRANO);
            string testString = "{\"Line\":5.0,\"AccidentalName\":\"\",\"Voice\":\"SOPRANO\",\"Value\":4,\"BarIndex\":1,\"VerticalIndex\":1}";
            Note parsedNote = NoteParser.ParseJsonToNote(testString).Note;

            Assert.Equal(testNote, parsedNote);
        }
    }
}
