namespace Algorithm.New.Music
{
    public enum ComponentType { Root, Second, Third, Fourth, Fifth, Sixth, Seventh, Ninth }
    public enum Alteration { Up, Down, None }

    public class Component
    {
        public ComponentType Type { get; private set; }
        public bool Obligatory { get; private set; }
        public Alteration Alteration { get; private set; }
        public bool Suspension { get; private set; }
        public bool Dissonant { get; private set; }

        public Component (ComponentType type, bool obligatory=false, Alteration alteration=Alteration.None, bool suspension=false, bool dissonant=false)
        {
            Type = type;
            Obligatory = obligatory;
            Alteration = alteration;
            Suspension = suspension;
            Dissonant = dissonant;
        }

        public static readonly Component Root = new(ComponentType.Root);
        public static readonly Component Second = new(ComponentType.Second, suspension: true, dissonant: true);
        public static readonly Component Third = new(ComponentType.Third, obligatory: true);
        public static readonly Component Fourth = new(ComponentType.Fourth, suspension: true, dissonant: true);
        public static readonly Component Fifth = new(ComponentType.Fifth);
        public static readonly Component Sixth = new(ComponentType.Sixth, dissonant: true);
        public static readonly Component Seventh = new(ComponentType.Seventh, dissonant: true);
        public static readonly Component Ninth = new(ComponentType.Ninth, dissonant: true);

        public static Component? GetByString(string value)
        {
            return value switch
            {
                "1" => Root,
                "2" => Second,
                "3" => Third,
                "4" => Fourth,
                "5" => Fifth,
                "6" => Sixth,
                "7" => Seventh,
                "9" => Ninth,
                _ => null
            };
        }
    }
}
