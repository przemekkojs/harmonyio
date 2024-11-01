namespace Algorithm.Old.Music
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

        public static readonly Meter Meter2_4 = new(2, 4);
        public static readonly Meter Meter3_4 = new(3, 4);
        public static readonly Meter Meter4_4 = new(4, 4);
        public static readonly Meter Meter3_8 = new(3, 2);
        public static readonly Meter Meter6_8 = new(6, 2);

        public static Meter GetMeter(int value, int count)
        {
            List<Meter> meters = [
                Meter2_4, Meter3_4, Meter4_4,
                Meter3_8, Meter6_8
            ];

            return meters.FirstOrDefault(x => x.Value == value && x.Count == count) ?? throw new ArgumentException("Invalid parameters.");
        }
    }
}
