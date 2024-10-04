namespace Algorithm.Music
{
    public class RhytmicValue
    {
        public int Duration { get => duration; }
        public string Name { get => name; }

        private readonly int duration;
        private readonly string name;

        public RhytmicValue(int duration)
        {
            this.duration = duration;

            DeductName();
        }

        private void DeductName()
        {

        }
    }
}
