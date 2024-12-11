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

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Metre casted)
                return casted.Count == Count && casted.Value == Value;
            else
                return false;
        }

        private void Validate()
        {
            if (Count < 0 || Value < 0)
                throw new ArgumentException("Invalid metre declatarion");
        }

        public static readonly Metre Meter2_4 = new(2, 4);
        public static readonly Metre Meter3_4 = new(3, 4);
        public static readonly Metre Meter4_4 = new(4, 4);
        public static readonly Metre Meter3_8 = new(3, 8);
        public static readonly Metre Meter6_8 = new(6, 8);

        public static Metre GetMetre(int count, int value)
        {
            List<Metre> meters = [
                Meter2_4, Meter3_4, Meter4_4,
                Meter3_8, Meter6_8
            ];

            var valueToMetreValue = value switch
            {
                2 => 8,
                4 => 4,
                _ => 2
            };

            return meters.FirstOrDefault(x => x.Value == value && x.Count == count) ?? Metre.Meter2_4;
        }
    }
}
