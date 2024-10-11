namespace Algorithm.Music
{
    public class Task
    {
        public List<Bar> Bars { get => bars; }
        public int Length { get => length; }

        private int length;
        private readonly List<Bar> bars;

        public Task(int length)
        {
            this.length = length;
            this.bars = new List<Bar>(length);
        }

        public void ChangeLength(int newLength)
        {
            if (newLength < length)
            {
                // TODO: What to do with unnecessary bars.
            }

            length = newLength;
        }
    }
}
