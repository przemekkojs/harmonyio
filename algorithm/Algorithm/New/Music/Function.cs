using Algorithm.New.Utils;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace Algorithm.New.Music
{
    public enum Symbol { T, Sii, Tiii, Diii, S, D, Tvi, Svi, Dvii, Svii }
    public enum InsertionType { Forward, Backward, None }
    
    public class ParsedFunction
    {
        public string Symbol { get; set; }
        public bool IsMain { get; set; }
        public bool Minor { get; set; }
        public string Root { get; set; }
        public string Position { get; set; }
        public string Removed { get; set; }
        public List<string> Added { get; set; }
        public List<string> Alterations { get; set; }
        public string BarIndex { get; set; }
        public string VerticalIndex { get; set; }

        [JsonConstructor]
        public ParsedFunction(bool minor, string symbol, string position, string root, string removed,
            List<string> alterations, List<string> added, string barIndex, string verticalIndex)
        {
            Minor = minor;
            Symbol = symbol;
            Position = position;
            Root = root;
            Removed = removed;
            Alterations = alterations;
            Added = added;
            BarIndex = barIndex;
            VerticalIndex = verticalIndex;
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
        
        public Function(ParsedFunction parsedFunction)
        {
            Index = new Index()
            {
                Bar = Convert.ToInt32(parsedFunction.BarIndex),
                Position = Convert.ToInt32(parsedFunction.VerticalIndex)
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
                _ => throw new ArgumentException("Invalid symbol.")
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

            // TODO - these should be also set somehow
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
            bool hasRoot = Root != null;
            bool hasPosition = Position != null;

            if (hasRoot)
            {
                if (Root.Equals(Component.Root) || Root.Equals(Component.Fifth))
                {
                    doubled.Add(Component.Root);
                    doubled.Add(Component.Fifth);
                }
                else if (Root.Equals(Component.Third))
                {
                    if (hasPosition)
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
                List<List<Component>> substitution = new();

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

            return true;
        }

        private static string ComponentListToString(List<Component> components) => "[" + string.Join(", ", components.ConvertAll(item => $"{item}")) + "]";
        public override string ToString() => $"{(Minor ? "m" : "")}{Symbol}^{Position}/{Root}+{ComponentListToString(Added)}+";
    }
}
