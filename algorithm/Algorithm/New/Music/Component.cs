using Algorithm.New.Algorithm;
using System.ComponentModel.DataAnnotations;

namespace Algorithm.New.Music
{
    public enum ComponentType { Root, Second, Third, Fourth, Fifth, Sixth, Seventh, Ninth }
    public enum Alteration { Up, Down, None }

    public class Component
    {
        public static readonly Dictionary<ComponentType, string> ComponentTypeToString = new()
        {
            { ComponentType.Root, "1" },
            { ComponentType.Second, "2" },
            { ComponentType.Third, "3" },
            { ComponentType.Fourth, "4" },
            { ComponentType.Fifth, "5" },
            { ComponentType.Sixth, "6" },
            { ComponentType.Seventh, "7" },
            { ComponentType.Ninth, "9" }
        };

        public ComponentType Type { get; private set; }
        public bool Obligatory { get; private set; }
        public Alteration Alteration { get; set; }
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

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Component casted)
            {
                var typeEqual = casted.Type == Type;
                var alterationEqual = casted.Alteration == Alteration;

                return typeEqual && alterationEqual;
            }

            return false;
        }

        public override string ToString()
        {
            return Type switch
            {
                ComponentType.Root => "1",
                ComponentType.Second => "2",
                ComponentType.Third => "3",
                ComponentType.Fourth => "4",
                ComponentType.Fifth => "5",
                ComponentType.Sixth => "6",
                ComponentType.Seventh => "7",
                _ => "9"
            };
        }
    }
}
