using System.Security.Cryptography;

namespace Algorithm.Music
{
    public class Function
    {
        public List<FunctionComponent> Components { get => components; }   
        public Symbol Symbol { get => symbol; }
        public int Duration { get => duration; }
        public int Beat { get => beat; }
        public FunctionType ChordType { get => chordType; }
        
        private readonly List<FunctionComponent> components;        
        private readonly Symbol symbol;

        private readonly int duration; //W jednostkach - z UI - slotach.
        private readonly int beat; // W jednostkach
        private readonly FunctionType chordType;

        public Function(Symbol symbol, int duration, int beat)
        {
            this.components = new();
            this.duration = duration;
            this.beat = beat;
            this.symbol = symbol;
            
            ValidateBeat();
            ValidateDuration();
            DeductFunctionComponents();
        }

        public void ValidateDuration()
        {
            if (!(new List<int>() { 1, 2, 3, 4, 6, 8, 12, 16 }.Contains(duration)))
                throw new ArgumentException("Invalid function duration");
        }

        public void ValidateBeat()
        {
            if (beat < 0)
                throw new ArgumentException("Invalid beat");
        }

        public void DeductFunctionComponents()
        {
            if (symbol.Root != null)
                components.Add(symbol.Root);

            if (symbol.Position != null)
                components.Add(symbol.Position);

            components.Add(FunctionComponent.Root);
            components.Add(FunctionComponent.Third);

            if (!symbol.Removed.Contains(FunctionComponent.Fifth))
                components.Add(FunctionComponent.Fifth);

            if (!symbol.Removed.Contains(FunctionComponent.Root))
                components.Add(FunctionComponent.Root);

            if (symbol.Added.Count > 0)
                components.AddRange(symbol.Added);

            if (components.Count > 4)
                components.Remove(FunctionComponent.Fifth);

            if (components.Count > 4)
                components.Remove(FunctionComponent.Root);

            if (components.Count < 4)
            {
                if (!symbol.Removed.Contains(FunctionComponent.Root))
                    components.Add(FunctionComponent.Root);
                else if (symbol.Added.Contains(FunctionComponent.Seventh))
                    components.Add(FunctionComponent.Seventh);
            }

            if (components.Count < 4)
                throw new ArgumentException("Invalid symbol.");
        }
    }
}
