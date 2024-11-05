using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Old.Music
{
    public enum AlterationSymbol
    {
        UP,
        DOWN
    }

    public class Alteration
    {
        public FunctionComponent ChordComponent { get => chordComponent; }
        public AlterationSymbol AlterationSymbol { get => symbol; }
        public int SemitonesChange { get => semitonesChange; }

        private readonly FunctionComponent chordComponent;
        private readonly AlterationSymbol symbol;
        private readonly int semitonesChange;

        public Alteration(FunctionComponent chordComponent, AlterationSymbol symbol)
        {
            this.chordComponent = chordComponent;
            this.symbol = symbol;

            switch (symbol)
            {
                case AlterationSymbol.UP:
                    semitonesChange = 1;
                    break;
                case AlterationSymbol.DOWN:
                    semitonesChange = -1;
                    break;
            }
        }
    }
}
