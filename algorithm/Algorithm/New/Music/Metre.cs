using System.Text.Json.Serialization;

namespace Algorithm.New.Music
{
    public class Metre
    {
        public int Count { get; set; }
        public int Value { get; set; }

        // TODO: Bind properties
        [JsonConstructor]
        public Metre(int count, int value)
        {
            Count = count;
            Value = value;
            Validate();
        }

        private void Validate()
        {
            if (Count < 0 ||  Value < 0)
                throw new ArgumentException("Invalid metre declatarion");
        }
    }
}
