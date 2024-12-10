using Algorithm.New.Utils;
using System.Text.Json.Serialization;

namespace Algorithm.New.Music
{
    public enum Symbol { T, Sii, Tiii, Diii, S, D, Tvi, Svi, Dvii, Svii, None }
    public enum InsertionType { Forward, Backward, None }

    public class ParsedFunction
    {
        public bool Minor { get; private set; }
        public string Symbol { get; private set; }
        public string Position { get; private set; }
        public string Root { get; private set; }
        public string Removed { get; private set; }
        public List<string> Alterations { get; private set; }
        public List<string> Added { get; private set; }
        public int BarIndex { get; private set; }
        public int VerticalIndex { get; private set; }

        [JsonConstructor]
        public ParsedFunction(
            bool minor,
            string symbol,
            string position,
            string root,
            string removed,
            List<string> alterations,
            List<string> added,
            int barIndex,
            int verticalIndex)
        {
            Minor = minor;
            Symbol = symbol;
            Position = position;
            Root = root;
            Removed = removed;
            Alterations = alterations ?? [];
            Added = added ?? [];
            BarIndex = barIndex;
            VerticalIndex = verticalIndex;
        }

        private ParsedFunction()
        {
            Symbol = string.Empty;
            Minor = false;
            Position = string.Empty;
            Root = string.Empty;
            Removed = string.Empty;
            Alterations = new List<string>();
            Added = new List<string>();
            BarIndex = 0;
            VerticalIndex = 0;
        }

        public static ParsedFunction CreateFromFunction(Function function)
        {
            var result = new ParsedFunction();

            result.Minor = function.Minor;
            result.Symbol = function.Symbol.ToString();
            result.Position = function.Position != null ?
                Component.ComponentTypeToString[function.Position.Type] :
                string.Empty;

            result.Root = function.Root != null ?
                Component.ComponentTypeToString[function.Root.Type] :
                string.Empty;

            result.Removed = function.Removed != null ?
                Component.ComponentTypeToString[function.Removed.Type] :
                string.Empty;

            // TODO: Zaimplementować, kiedy alteracje się pojawią w klasie Function
            result.Alterations = [];
            result.Added = [];

            foreach (var added in function.Added)
            {
                var toAdd = added != null ?
                    Component.ComponentTypeToString[added.Type] :
                    string.Empty;

                result.Added.Add(toAdd); // AddAddAddAddAddAddAddAddAddAddAddAddAddAddAddAddAddAddAddAddAdd
            }

            result.BarIndex = function.Index.Bar;
            result.VerticalIndex = function.Index.Position;

            return result;
        }
    }

    public class Function
    {
        public Symbol Symbol { get; set; }
        public bool IsMain { get; set; }
        public bool Minor { get; set; }
        public Component? Root { get; set; }
        public Component? Position { get; set; }
        public Component? Removed { get; set; }
        public List<Component> Added { get; set; }
        public List<List<Component>> PossibleComponents { get; set; }
        public Tonation Tonation { get; set; }
        public bool IsInsertion { get; set; }
        public InsertionType InsertionType { get; set; }
        public Index Index { get; set; }

        public static Dictionary<Component, string> FunctionComponentIndexes(int startIndex, Tonation tonation)
        {
            int Offset(int offset) => (offset + startIndex) % Constants.NOTES_IN_TONATION;
            List<string> tonationNotes = Utils.Scale.Notes(tonation);

            return new Dictionary<Component, string>()
            {
                { Component.Root, tonationNotes[Offset(0)] },
                { Component.Second, tonationNotes[Offset(1)] },
                { Component.Third, tonationNotes[Offset(2)] },
                { Component.Fourth, tonationNotes[Offset(3)] },
                { Component.Fifth, tonationNotes[Offset(4)] },
                { Component.Sixth, tonationNotes[Offset(5)] },
                { Component.Seventh, tonationNotes[Offset(6)] },
                { Component.Ninth, tonationNotes[Offset(1)] }
            };
        }

        public static readonly Dictionary<Symbol, int> SymbolIndexes = new()
        {
            { Symbol.T, 0 },
            { Symbol.Sii, 1 },
            { Symbol.Tiii, 2 },
            { Symbol.Diii, 2 },
            { Symbol.S, 3 },
            { Symbol.D, 4 },
            { Symbol.Tvi, 5 },
            { Symbol.Svi, 5 },
            { Symbol.Dvii, 6 },
            { Symbol.Svii, 6 }
        };

        public static readonly Dictionary<Symbol, bool> MainMapping = new()
        {
            { Symbol.T, true },
            { Symbol.Sii, false },
            { Symbol.Tiii, false },
            { Symbol.Diii, false },
            { Symbol.S, true },
            { Symbol.D, true },
            { Symbol.Tvi, false },
            { Symbol.Svi, false },
            { Symbol.Dvii, false },
            { Symbol.Svii, false }
        };

        /// <summary>WAŻNE! Ustawić Tonation osobno przy wykorzystaniu tego konstruktora</summary>
        public Function(ParsedFunction parsedFunction)
        {
            Index = new Index()
            {
                Bar = parsedFunction.BarIndex,
                Position = parsedFunction.VerticalIndex
            };

            Symbol = parsedFunction.Symbol switch
            {
                "T" => Symbol.T,
                "Sii" => Symbol.Sii,
                "Tiii" => Symbol.Tiii,
                "Diii" => Symbol.Diii,
                "S" => Symbol.S,
                "D" => Symbol.D,
                "Tvi" => Symbol.Tvi,
                "Svi" => Symbol.Svi,
                "Dvii" => Symbol.Dvii,
                "Svii" => Symbol.Svii,
                _ => Symbol.None
            };

            IsMain = new List<string>() { "T", "S", "D" }.Contains(parsedFunction.Symbol);

            Minor = parsedFunction.Minor;

            Root = Component.GetByString(parsedFunction.Root);
            Position = Component.GetByString(parsedFunction.Position);
            Removed = Component.GetByString(parsedFunction.Removed);

            Added = [];

            foreach (var addedString in parsedFunction.Added)
            {
                var toAdd = Component.GetByString(addedString);

                if (toAdd != null)
                    Added.Add(toAdd);
            }

            foreach (var alterationString in parsedFunction.Alterations)
            {
                var componentToAlterString = alterationString[0].ToString();
                var alterationType = alterationString[1..] ?? "";

                var component = Component.GetByString(componentToAlterString);

                if (component == null)
                    throw new ArgumentException("Cannot add alteration to non-existend component");

                component.Alteration = alterationType.Equals(">") ? Alteration.Down : Alteration.Up;
            }

            PossibleComponents = [];

            InsertionType = InsertionType.None;
            IsInsertion = false;
            Tonation = Tonation.CMajor;

            DeductPossibleComponents2();
        }

        public Function(Index index, Symbol symbol, bool minor, Tonation tonation,
            bool isInsertion = false, InsertionType insertionType = InsertionType.None,
            Component? root = null, Component? position = null, Component? removed = null,
            List<string>? alterations = null, List<Component>? added = null)
        {
            Index = index;
            Symbol = symbol;
            Minor = minor;
            Root = root;
            Position = position;
            Removed = removed;
            Tonation = tonation;
            IsInsertion = isInsertion;
            InsertionType = insertionType;

            IsMain = MainMapping[symbol];

            Added = added ?? [];

            PossibleComponents = [];

            DeductPossibleComponents2();
        }

        private void DeductPossibleComponents2()
        {
            if (!IsMain && Added.Contains(Component.Sixth))
                throw new ArgumentException("Cannot add sixth to non-main function.");

            if (Added.Count > 2)
                throw new ArgumentException("Cannot have more than 2 added components at once");

            if (Added.Contains(Component.Seventh) && Added.Contains(Component.Sixth))
            {
                if (Minor)
                    throw new ArgumentException("Cannot create 7 add6 minor function.");

                if (Symbol != Symbol.D)
                    throw new ArgumentException("Cannot create 7 add6 function other than Dominant.");

                PossibleComponents = [[Component.Root, Component.Third, Component.Sixth, Component.Seventh]];
            }

            if (Added.Contains(Component.Ninth))
                Added.Add(Component.Seventh);

            // Bazowo zawsze będzie 1, 3, 5
            var template = new List<Component>()
            {
                Component.Root,
                Component.Third,
                Component.Fifth
            };            

            // Bazowo w głównych podwajamy (1 albo 5), w pobocznych (1 albo 3)
            List<Component> toDouble = IsMain?
                [Component.Root, Component.Fifth] :
                [Component.Root, Component.Third];

            if (Root != null && Position != null)
            {
                if (Root.Equals(Component.Ninth))
                    throw new ArgumentException("Cannot have fifth as root");

                // Nie można usunąć tego, co jest w pozycji / oparciu
                if (Root.Equals(Removed) || Position.Equals(Removed))
                    throw new ArgumentException("Cannot create function like that");

                if (Root.Equals(Component.Sixth) || Root.Equals(Component.Seventh))
                {
                    if (!Added.Contains(Root))
                        throw new ArgumentException("Cannot have non-added component as root");
                }

                if (Position.Equals(Component.Sixth) || Position.Equals(Component.Seventh) || Position.Equals(Component.Ninth))
                {
                    if (!Added.Contains(Position))
                        throw new ArgumentException("Cannot have non-added component as position");
                }

                if (Root.Equals(Position))
                {
                    var componentToDouble = Root;
                    toDouble = [componentToDouble];
                }
                else
                {

                }
            }

            if (Added.Contains(Component.Seventh))
                toDouble.Remove(Component.Third);

            // Dodajemy od razu wszystkie składniki dodane
            foreach (var added in Added)
            {
                template.Add(added);

                if (added.Equals(Component.Seventh))
                    toDouble.Add(added);
            }

            // Sprawdzamy, czy czegoś nie możemy usunąć
            if (Removed != null)
            {
                if (Added.Count == 0)
                {
                    PossibleComponents = IsMain ?
                    [
                        [
                            Component.Root,
                            Component.Root,
                            Component.Root,
                            Component.Third
                        ]
                    ] :
                    [
                        [
                            Component.Root,
                            Component.Root,
                            Component.Root,
                            Component.Third
                        ],
                        [
                            Component.Root,
                            Component.Root,
                            Component.Third,
                            Component.Third
                        ]
                    ];
                    return;
                }

                if (Removed.Equals(Component.Root))
                {
                    template.Remove(Component.Root);
                    toDouble.Remove(Component.Root);
                }
                else // tutaj tylko kwinta będzie możliwa
                {
                    template.Remove(Component.Fifth);
                    toDouble.Remove(Component.Fifth);
                }
            }            
            
            if (Added.Count == 2)
            {
                if (Removed == null)
                    template.Remove(Component.Fifth);

                toDouble.Clear();
            }

            var actual = new List<List<Component>>();

            if (template.Count != Constants.NOTES_IN_FUNCTION)
            {
                foreach (var doubled in toDouble)
                {
                    var copy = new List<Component>(template) { doubled };
                    actual.Add(copy);
                }
            }
            else
                actual.Add(template);

            PossibleComponents = actual;

            ValidatePossibleComponents();
        }

        private void ValidatePossibleComponents()
        {
            // zawsze mogą być tylko 4. Wszelkie mniej lub więcej trzeba wywalić.
            PossibleComponents
                .RemoveAll(x => x.Count != Constants.NOTES_IN_FUNCTION);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Function casted)
            {
                var minorEqual = (casted.Minor == Minor);
                var symbolEqual = (casted.Symbol == Symbol);
                var addedEqual = (casted.Added.SequenceEqual(Added));
                var removedEqual = (casted.Removed?.Equals(Removed)) ?? Removed == null;
                var rootEquals = (casted.Root?.Equals(Root)) ?? Root == null;
                var positionEquals = (casted.Position?.Equals(Position)) ?? Position == null;
                var insertionEqual = (casted.IsInsertion == IsInsertion);
                var insertionTypeEqual = (casted.InsertionType == InsertionType);

                return minorEqual && symbolEqual && addedEqual &&
                    removedEqual && rootEquals && positionEquals &&
                    insertionEqual && insertionTypeEqual;
            }
            else
                return false;
        }

        private static string ComponentListToString(List<Component> components) => "[" + string.Join(", ", components.ConvertAll(item => $"{item}")) + "]";
        public override string ToString() => $"{(Minor ? "m" : "")}{Symbol}^{Position}/{Root}+{ComponentListToString(Added)}+";
    }
}
