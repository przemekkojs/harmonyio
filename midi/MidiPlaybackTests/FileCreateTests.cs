using MidiPlayback;

namespace MidiPlaybackTests
{
    public class FileCreateTests
    {
        [Fact]
        public void Test1()
        {
            var create = FileCreator.Create();
            Assert.True(create.Length > 0);
        }
    }
}
