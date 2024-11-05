using System.Text.RegularExpressions;

namespace Algorithm.Old.Music
{
    public enum FunctionSymbol
    {
        T,
        Sii,
        Tiii,
        Diii,
        S,
        D,
        Tvi,
        Svi,
        Dvii,
        Svii
    }

    public class Symbol
    {
        public bool Minor { get => minor; }
        public FunctionSymbol FunctionSymbol { get => functionSymbol; }
        public List<(FunctionComponent, FunctionComponent)> Suspensions { get => suspensions ?? ([]); }
        public List<Alteration> Alterations { get => alterations ?? ([]); }
        public List<FunctionComponent> Added { get => added ?? ([]); }
        public FunctionComponent? Removed { get => removed; }
        public FunctionComponent? Position { get => position; }
        public FunctionComponent? Root { get => root; }

        private readonly FunctionSymbol functionSymbol;

        private readonly List<Alteration>? alterations;

        private readonly List<(FunctionComponent, FunctionComponent)>? suspensions;

        private readonly List<FunctionComponent>? added;
        private readonly FunctionComponent? removed;

        private readonly FunctionComponent? position;
        private readonly FunctionComponent? root;

        private readonly bool minor;

        public Symbol(bool minor, FunctionSymbol functionSymbol, FunctionComponent? root = null, FunctionComponent? position = null, List<FunctionComponent>? added = default, FunctionComponent? removed = null, List<(FunctionComponent, FunctionComponent)>? suspensions = default, List<Alteration>? alterations = default)
        {
            this.functionSymbol = functionSymbol;
            this.root = root;
            this.position = position;
            this.added = added;
            this.removed = removed;
            this.alterations = alterations;
            this.suspensions = suspensions;
            this.minor = minor;

            ValidateAdded();
            ValidateRemoved();
            ValidateAlterations();
            ValidateSuspensions();
            ValidatePosition();
            ValidateRoot();
        }

        public void ValidateSuspensions()
        {
            if (Suspensions.Count > 4)
                throw new ArgumentException("Too many suspensions.");

            foreach (var suspension in Suspensions)
            {
                if (suspension.Item1.Equals(suspension.Item2))
                    throw new ArgumentException("Suspensions cannot be the same across functions.");
            }
        }

        public void ValidateAlterations()
        {
            if (Alterations.Count > 4)
                throw new ArgumentException("Too many alterations.");
        }

        public void ValidateAdded()
        {
            int modifier = Removed != null ? 1 : 0;

            if (Added.Count - modifier > 2)
                throw new ArgumentException("Too many added components.");
        }

        public void ValidateRemoved()
        {
            int modifier = Removed != null ? 1 : 0;

            if (modifier - Added.Count > 2)
                throw new ArgumentException("Too many removed components.");
        }

        public void ValidatePosition()
        {

        }

        public void ValidateRoot()
        {
            if (root == null)
                return;

            if (root.Equals(FunctionComponent.Ninth))
                throw new ArgumentException("Function cannot have ninth as root.");
        }
    }
}
