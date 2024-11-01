using Algorithm.New.Utils;
using System.Text.Json.Serialization;

namespace Algorithm.New.Music
{
    public enum Symbol { T, Sii, Tiii, Diii, S, D, Tvi, Svi, Dvii, Svii }
    public enum InsertionType { Forward, Backward, None }
    
    public class Function
    {
        public Symbol Symbol { get; set; }
        public bool IsMain { get; set; }
        public bool Minor { get; set; }
        public Component? Root { get; set; }
        public Component? Position { get; set; }
        public Component? Removed { get; set; }
        public List<Component> Alterations { get; set; }
        public List<Component> Added { get; set; }
        public List<List<Component>> PossibleComponents { get; set; }
        public Tonation Tonation { get; set; }
        public bool IsInsertion { get; set; }
        public InsertionType InsertionType { get; set; }

        // TODO IN LATER VERSION: Add suspensions
        
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

        // TODO: Params....
        public Function(Index index, Symbol symbol, bool minor, Tonation tonation,
            bool isInsertion=false, InsertionType insertionType=InsertionType.None,
            Component? root=null, Component? position=null, Component? removed=null,
            List<Component>? alterations=null, List<Component>? added=null)
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

            Alterations = alterations ?? [];
            Added = added ?? [];

            PossibleComponents = [];

            DeductPossibleComponents();
        }

        // TODO: Re-think JSON constructor
        [JsonConstructor]
        public Function(bool minor, string symbol, string position, string root, string removed,
            List<string> alterations, List<string> added, int barIndex, int verticalIndex)
        {
            Index = new Index()
            { 
                Bar = barIndex,
                Beat = verticalIndex
            };

            Symbol = symbol switch
            {
                "T" => Symbol.T,
                "Sii" => Symbol.Sii,
                "Diii" => Symbol.Diii,
                "S" => Symbol.S,
                "D" => Symbol.D,
                "Tvi" => Symbol.Tvi,
                "Svi" => Symbol.Svi,
                "Dvii" => Symbol.Dvii,
                "Svii" => Symbol.Svii,
                _ => throw new ArgumentException("Invalid symbol.")
            };

            Minor = minor;

            Root = Component.GetByString(root);
            Position = Component.GetByString(position);
            Removed = Component.GetByString(removed);

            Alterations = [];
            Added = [];

            foreach (var alterationString in alterations)
            {
                var toAdd = Component.GetByString(alterationString);
                
                if (toAdd != null)
                    Alterations.Add(toAdd);
            }
                

            foreach (var addedString in added)
            {
                var toAdd = Component.GetByString(addedString);

                if (toAdd != null)
                    Alterations.Add(toAdd);
            }

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
    }
}
