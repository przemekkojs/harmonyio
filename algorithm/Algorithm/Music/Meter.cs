namespace Algorithm.Music
{
    public class Meter
    {
        public int Count { get => count; }
        public int Value { get => value; }

        private readonly int count;
        private readonly int value;

        public Meter(int count, int value)
        {
            this.count = count;
            this.value = value;
        }
    }
}
