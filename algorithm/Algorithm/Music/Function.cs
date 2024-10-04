using System.Security.Cryptography;

namespace Algorithm.Music
{
    public class Function
    {
        public List<(FunctionComponent, FunctionComponent)> Suspensions { get => suspensions; }
        public List<Alteration> Alterations { get => alterations; }
        public List<FunctionComponent> Added { get => added; }
        public List<FunctionComponent> Removed { get => removed; }
        public List<FunctionComponent> Components { get => components; }
        public FunctionComponent Position { get => position; }
        public FunctionComponent Root { get => root; }
        public Symbol Symbol { get => symbol; }
        public int Duration { get => duration; }
        public int Beat { get => beat; }
        public FunctionType ChordType { get => chordType; }

        private readonly List<Alteration> alterations;
        private readonly List<(FunctionComponent, FunctionComponent)> suspensions;
        private readonly List<FunctionComponent> components;
        private readonly List<FunctionComponent> added;
        private readonly List<FunctionComponent> removed;

        private readonly FunctionComponent position;
        private readonly FunctionComponent root;
        private readonly Symbol symbol;

        private readonly int duration; //W jednostkach - z UI - slotach.
        private readonly int beat; // W jednostkach
        private readonly FunctionType chordType;

        public Function(FunctionType chordType, FunctionComponent root, FunctionComponent position, List<(FunctionComponent, FunctionComponent)> suspensions, List<Alteration> alterations, List<FunctionComponent> added, List<FunctionComponent> removed, int duration, int beat)
        {
            this.components = new();
            this.chordType = chordType;
            this.suspensions = suspensions;
            this.alterations = alterations;
            this.added = added;
            this.removed = removed;
            this.root = root;
            this.position = position;
            this.duration = duration;
            this.beat = beat;

            DeductSymbol();
            ValidateAdded();
            ValidateRemoved();
            ValidateAlterations();
            ValidateSuspensions();
            ValidateBeat();
            ValidateDuration();
        }

        public Function(Symbol symbol, int duration, int beat)
        {
            this.symbol = symbol;
            this.components = new();
            this.added = new();
            this.removed = new();
            this.suspensions = new();
            this.alterations = new();
            this.duration = duration;
            this.beat = beat;

            DeductFunctionComponents();
            ValidateBeat();
            ValidateDuration();
        }

        public void ValidateDuration()
        {

        }

        public void ValidateBeat()
        {

        }

        public void ValidateSuspensions()
        {

        }

        public void ValidateAlterations()
        {

        }

        public void ValidateAdded()
        {

        }

        public void ValidateRemoved()
        {

        }

        public void DeductSymbol()
        {

        }

        public void DeductFunctionComponents()
        {

        }
    }
}
