using Algorithm.Utils;

namespace Algorithm.Music
{
    public class Function
    {
        public List<List<FunctionComponent>> Components { get => components; }   
        public Symbol Symbol { get => symbol; }
        public FunctionType ChordType { get => chordType; }
        public bool IsMain { get => isMain; }
        
        private readonly List<List<FunctionComponent>> components;        
        private readonly Symbol symbol;

        private readonly FunctionType chordType;
        private readonly bool isMain;        

        public Function(Symbol symbol, bool isMain)
        {
            this.components = [];
            this.symbol = symbol;
            this.isMain = isMain;
            
            DeductFunctionComponents();
        }
                    
        public void DeductFunctionComponents()
        {
            List<FunctionComponent> doubled = [];
            List<FunctionComponent> added = [];

            DeductRootAndPosition(doubled);
            DeductAdded(added, doubled);
            CreateComponents(added, doubled);
            ValidateComponents();
        }

        private void DeductRootAndPosition(List<FunctionComponent> doubled)
        {
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
        }

        private void DeductAdded(List<FunctionComponent> added, List<FunctionComponent> doubled)
        {
            if (symbol.Added.Count != 0)
            {
                if (symbol.Added.Contains(FunctionComponent.Seventh))
                {
                    if (symbol.Removed != null)
                    {
                        if (symbol.Removed.Equals(FunctionComponent.Root))
                        {
                            doubled.Remove(FunctionComponent.Root);
                            doubled.Remove(FunctionComponent.Fifth);
                        }
                        else if (symbol.Removed.Equals(FunctionComponent.Fifth))
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
                        if (symbol.Removed.Equals(FunctionComponent.Root))
                        {
                            doubled.Remove(FunctionComponent.Root);
                        }
                        else if (symbol.Removed.Equals(FunctionComponent.Fifth))
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
                    added.Add(FunctionComponent.Seventh);
                    doubled.Remove(FunctionComponent.Fifth);
                }
                else
                {
                    throw new ArgumentException("Invalid symbol.");
                }
            }
        }

        private void CreateComponents(List<FunctionComponent> added, List<FunctionComponent> doubled)
        {
            List<FunctionComponent> innerListTemplate = [];
            innerListTemplate.Add(FunctionComponent.Third);

            foreach (var possibleAdded in added)
            {
                innerListTemplate.Add(possibleAdded);
                Components.Add(innerListTemplate);
            }

            int missing = Constants.Constants.NOTES_IN_FUNCTION - innerListTemplate.Count;

            if (missing <= doubled.Count)
            {
                var perms = Permutations.CreatePermutations<FunctionComponent>(doubled, missing);

                foreach (var perm in perms)
                {
                    var copy = new List<FunctionComponent>(innerListTemplate);
                    copy.AddRange(perm);
                    Components.Add(copy);
                }
            }
            else
            {
                var perms = Permutations.CreatePermutations<FunctionComponent>(doubled, doubled.Count);

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
        }

        private void ValidateComponents()
        {
            components.RemoveAll(x => (x.Count != Constants.Constants.NOTES_IN_FUNCTION));
        }
    }
}
