namespace Algorithm.Old.Music
{
    public class Task
    {
        public Tonation Tonation { get => tonation; }
        public List<Bar> Bars { get => bars; }
        public int Length { get; set; }

        private readonly List<Bar> bars;
        private readonly Tonation tonation;

        public Task(int length, Tonation tonation)
        {
            Length = length;
            bars = new List<Bar>(length);
            this.tonation = tonation;
        }

        public void ChangeLength(int newLength)
        {
            if (newLength < Length)
            {
                // TODO: What to do with unnecessary bars.
            }

            Length = newLength;
        }

        public void AddBar(Bar bar)
        {
            if (bars.Count < Length)
                bars.Add(bar);
            else
                throw new ArgumentException("Cannot add more bars");
        }

        public void AddBarsRange(IEnumerable<Bar> bars)
        {
            foreach (Bar bar in bars)
            {
                AddBar(bar);
            }
        }
    }
}
