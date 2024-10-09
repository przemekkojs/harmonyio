using Microsoft.VisualBasic.FileIO;
using System.Security.Cryptography;

namespace Algorithm.Music
{
    public class Function
    {
        public List<List<FunctionComponent>> Components { get => components; }   
        public Symbol Symbol { get => symbol; }
        public int Duration { get => duration; }
        public int Beat { get => beat; }
        public FunctionType ChordType { get => chordType; }
        public bool IsMain { get => isMain; }
        
        private readonly List<List<FunctionComponent>> components;        
        private readonly Symbol symbol;

        private readonly int duration; //W jednostkach - z UI - slotach.
        private readonly int beat; // W jednostkach
        private readonly FunctionType chordType;
        private readonly bool isMain;

        public Function(Symbol symbol, bool isMain, int duration, int beat)
        {
            this.components = new();
            this.duration = duration;
            this.beat = beat;
            this.symbol = symbol;
            this.isMain = isMain;
            
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
            List<FunctionComponent> doubled = [];
            List<FunctionComponent> added = [];

            bool hasRoot = (symbol.Root != null);
            bool hasPosition = (symbol.Position != null);

            if (hasRoot)
            {
                if (symbol.Root.Equals(FunctionComponent.Root) || symbol.Root.Equals(FunctionComponent.Fifth))
                {
                    doubled.Add(FunctionComponent.Root);
                    doubled.Add(FunctionComponent.Fifth);
                }
                else if (symbol.Root.Equals(FunctionComponent.Third))
                {
                    if (hasPosition)
                    {
                        if (symbol.Position.Equals(FunctionComponent.Root))
                            doubled.Add(FunctionComponent.Root);
                        else if (symbol.Position.Equals(FunctionComponent.Fifth))
                            doubled.Add(FunctionComponent.Fifth);
                        else if (symbol.Position.Equals(FunctionComponent.Third))
                        {
                            if (isMain)
                            {
                                doubled.Add(FunctionComponent.Root);
                                doubled.Add(FunctionComponent.Third);
                                doubled.Add(FunctionComponent.Fifth);
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
                if (isMain)
                {
                    doubled.Add(FunctionComponent.Root);
                    doubled.Add(FunctionComponent.Fifth);
                }
                else
                {
                    doubled.Add(FunctionComponent.Root);
                    doubled.Add(FunctionComponent.Third);
                    doubled.Add(FunctionComponent.Fifth);
                }
            }

            if (symbol.Added.Count != 0)
            {
                if (symbol.Added.Contains(FunctionComponent.Seventh))
                {
                    if (symbol.Removed != null)
                    {
                        if (symbol.Removed.Contains(FunctionComponent.Root))
                        {
                            doubled.Remove(FunctionComponent.Root);
                            doubled.Remove(FunctionComponent.Fifth);
                        }
                        else if (symbol.Removed.Contains(FunctionComponent.Fifth))
                        {
                            doubled.Remove(FunctionComponent.Fifth);
                        }

                        doubled.Add(FunctionComponent.Seventh);
                    }
                }
                else if (symbol.Added.Contains(FunctionComponent.Ninth))
                {
                    if (symbol.Removed != null)
                    {
                        if (symbol.Removed.Contains(FunctionComponent.Root))
                        {
                            doubled.Remove(FunctionComponent.Root);
                        }
                        else if (symbol.Removed.Contains(FunctionComponent.Fifth))
                        {
                            doubled.Remove(FunctionComponent.Fifth);
                        }

                        added.Add(FunctionComponent.Seventh);
                        added.Add(FunctionComponent.Ninth);
                    }
                }
                else if (symbol.Added.Contains(FunctionComponent.Sixth))
                {
                    added.Add(FunctionComponent.Sixth);                    
                }
                else
                {
                    throw new ArgumentException("Invalid symbol.");
                }                
            }

            List<FunctionComponent> innerListTemplate = [];
            innerListTemplate.Add(FunctionComponent.Third);

            foreach (var possibleAdded in added)
            {
                innerListTemplate.Add(possibleAdded);
                Components.Add(innerListTemplate);
            }

            int missing = 4 - innerListTemplate.Count;

            if (missing <= doubled.Count)
            {
                var perms = CreatePermutations<FunctionComponent>(doubled, missing);

                foreach (var perm in perms)
                {
                    var copy = new List<FunctionComponent>(innerListTemplate);
                    copy.AddRange(perm);
                    Components.Add(copy);
                }
            }
            else
            {                
                var perms = CreatePermutations<FunctionComponent>(doubled, doubled.Count);

                foreach (var perm in perms)
                {
                    var copy = new List<FunctionComponent>(innerListTemplate);
                    copy.AddRange(perm);
                    Components.Add(copy);
                }

                // Only one will always remain, so another set of permutations can be added
                List<List<FunctionComponent>> substitution = new();

                foreach (var componentList in Components)
                {
                    foreach (var possibleDoubled in doubled)
                    {
                        var copy = new List<FunctionComponent>(componentList) { possibleDoubled };
                        substitution.Add(copy);
                    }
                }

                components.Clear();
                components.AddRange(substitution);
            }

            components.RemoveAll(x => (x.Count != 4));
        }

        private static List<List<T>> CreatePermutations<T>(List<T> set, int n)
        {
            var result = new List<List<T>>();
            CreatePermutationsRecursive(set, new List<T>(), result, n, 0);
            return result;
        }

        private static void CreatePermutationsRecursive<T>(List<T> set, List<T> current, List<List<T>> result, int n, int startIndex)
        {
            if (current.Count == n)
            {
                result.Add(new List<T>(current));
                return;
            }

            for (int i = startIndex; i < set.Count; i++)
            {
                current.Add(set[i]);
                CreatePermutationsRecursive(set, current, result, n, i + 1);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}
