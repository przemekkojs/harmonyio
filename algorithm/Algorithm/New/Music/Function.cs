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
                // TODO: Co z '<' w np. 7< czy 6<...

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

        public static readonly Dictionary<Symbol, int> symbolIndexes = new()
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
            { Symbol.Svii, 6 },
        };

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
                // TODO: Co z '<', '>', ...
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

            DeductPossibleComponents();
        }

        // TODO: Params....
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

            Added = added ?? [];

            PossibleComponents = [];

            DeductPossibleComponents();
        }

        public void DeductPossibleComponents()
        {
            List<Component> doubled = [];
            List<Component> added = [];

            DeductRootAndPosition(doubled);
            DeductAdded(added, doubled);
            CreatePossibleComponents(added, doubled);
            ValidatePossibleComponents();
        }

        private void DeductRootAndPosition(List<Component> doubled)
        {
            if (Root != null)
            {
                if (Root.Equals(Component.Root) || Root.Equals(Component.Fifth))
                {
                    doubled.Add(Component.Root);
                    doubled.Add(Component.Fifth);
                }
                else if (Root.Equals(Component.Third))
                {
                    if (Position != null)
                    {
                        if (Position.Equals(Component.Root))
                            doubled.Add(Component.Root);
                        else if (Position.Equals(Component.Fifth))
                            doubled.Add(Component.Fifth);
                        else if (Position.Equals(Component.Third))
                        {
                            if (IsMain)
                            {
                                doubled.Add(Component.Root);
                                doubled.Add(Component.Third);
                                doubled.Add(Component.Fifth);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid function symbol.");
                            }
                        }
                    }
                }
            }
            else
            {
                if (IsMain)
                {
                    doubled.Add(Component.Root);
                    doubled.Add(Component.Fifth);
                }
                else
                {
                    doubled.Add(Component.Root);
                    doubled.Add(Component.Third);
                    doubled.Add(Component.Fifth);
                }
            }
        }

        private void DeductAdded(List<Component> added, List<Component> doubled)
        {
            if (Added.Count != 0)
            {
                if (Added.Contains(Component.Seventh))
                {
                    if (Removed != null)
                    {
                        if (Removed.Equals(Component.Root))
                        {
                            doubled.Remove(Component.Root);
                            doubled.Remove(Component.Fifth);
                        }
                        else if (Removed.Equals(Component.Fifth))
                        {
                            doubled.Remove(Component.Fifth);
                        }

                        doubled.Add(Component.Seventh);
                    }
                    else
                    {
                        added.Add(Component.Seventh);
                    }
                }
                else if (Added.Contains(Component.Ninth))
                {
                    if (Removed != null)
                    {
                        if (Removed.Equals(Component.Root))
                        {
                            doubled.Remove(Component.Root);
                        }
                        else if (Removed.Equals(Component.Fifth))
                        {
                            doubled.Remove(Component.Fifth);
                        }

                        added.Add(Component.Seventh);
                        added.Add(Component.Ninth);
                    }
                }
                else if (Added.Contains(Component.Sixth))
                {
                    added.Add(Component.Sixth);
                    added.Add(Component.Seventh);
                    doubled.Remove(Component.Fifth);
                }
                else
                {
                    throw new ArgumentException("Invalid symbol.");
                }
            }
        }

        private void CreatePossibleComponents(List<Component> added, List<Component> doubled)
        {
            List<Component> innerListTemplate = [];
            innerListTemplate.Add(Component.Third);

            foreach (var possibleAdded in added)
            {
                innerListTemplate.Add(possibleAdded);
                PossibleComponents.Add(innerListTemplate);
            }

            int missing = Constants.NOTES_IN_FUNCTION - innerListTemplate.Count;

            if (missing <= doubled.Count)
            {
                var perms = Permutations.CreatePermutations(doubled, missing);

                foreach (var perm in perms)
                {
                    var copy = new List<Component>(innerListTemplate);
                    copy.AddRange(perm);
                    PossibleComponents.Add(copy);
                }
            }
            else
            {
                var perms = Permutations.CreatePermutations(doubled, doubled.Count);

                foreach (var perm in perms)
                {
                    var copy = new List<Component>(innerListTemplate);
                    copy.AddRange(perm);
                    PossibleComponents.Add(copy);
                }

                // Only one will always remain, so another set of permutations can be added
                List<List<Component>> substitution = [];

                foreach (var componentList in PossibleComponents)
                {
                    foreach (var possibleDoubled in doubled)
                    {
                        var copy = new List<Component>(componentList) { possibleDoubled };
                        substitution.Add(copy);
                    }
                }

                PossibleComponents.Clear();
                PossibleComponents.AddRange(substitution);
            }
        }

        private void ValidatePossibleComponents()
        {
            PossibleComponents.RemoveAll(x => x.Count != Constants.NOTES_IN_FUNCTION);
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
