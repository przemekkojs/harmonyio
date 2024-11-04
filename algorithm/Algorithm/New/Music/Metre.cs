using System.Text.Json.Serialization;

namespace Algorithm.New.Music
{
    public class Metre
    {
        [JsonPropertyName("metreCount")] // Na potrzeby kodu Kuby
        public int Count { get; set; }

        [JsonPropertyName("metreValue")] // Na potrzeby kodu Kuby
        public int Value { get; set; }

        [JsonConstructor]
        public Metre(int metreCount, int metreValue)
        {
            Count = metreCount;
            Value = metreValue;

            Validate();
        }

        private void Validate()
        {
            if (Count < 0 ||  Value < 0)
                throw new ArgumentException("Invalid metre declatarion");
        }

        public static readonly Metre Meter2_4 = new(2, 4);
        public static readonly Metre Meter3_4 = new(3, 4);
        public static readonly Metre Meter4_4 = new(4, 4);
        public static readonly Metre Meter3_8 = new(3, 2);
        public static readonly Metre Meter6_8 = new(6, 2);

        public static Metre GetMetre(int count, int value)
        {
            List<Metre> meters = [
                Meter2_4, Meter3_4, Meter4_4,
                Meter3_8, Meter6_8
            ];

            return meters.FirstOrDefault(x => x.Value == value && x.Count == count) ?? throw new ArgumentException("Invalid metre parameters.");
        }
    }
}
