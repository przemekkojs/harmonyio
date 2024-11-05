namespace Algorithm.Old.Music
{
    public enum Component
    {
        ROOT,
        SECOND,
        THIRD,
        FOURTH,
        FIFTH,
        SIXTH,
        SEVENTH,
        NINTH,
        ELEVENTH
    }

    public class FunctionComponent
    {
        public Component Symbol { get => symbol; }
        public bool Required { get => required; }
        public bool Suspension { get => suspension; }

        private readonly Component symbol;
        private readonly bool required;
        private readonly bool suspension;
        private readonly bool dissonant;

        public FunctionComponent(Component symbol, bool required = false, bool suspension = false, bool dissonant = false)
        {
            this.symbol = symbol;
            this.required = required;
            this.suspension = suspension;
            this.dissonant = dissonant;
        }

        public static readonly FunctionComponent Root = new(Component.ROOT);
        public static readonly FunctionComponent Second = new(Component.SECOND, suspension: true, dissonant: true);
        public static readonly FunctionComponent Third = new(Component.THIRD, required: true);
        public static readonly FunctionComponent Fourth = new(Component.FOURTH, suspension: true, dissonant: true);
        public static readonly FunctionComponent Fifth = new(Component.FIFTH);
        public static readonly FunctionComponent Sixth = new(Component.SIXTH, dissonant: true);
        public static readonly FunctionComponent Seventh = new(Component.SEVENTH, dissonant: true);
        public static readonly FunctionComponent Ninth = new(Component.NINTH, dissonant: true);
        public static readonly FunctionComponent Eleventh = new(Component.ELEVENTH, dissonant: true);
    }
}
