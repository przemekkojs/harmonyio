using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Music
{
    public class Ellipse
    {
        public Function SecondaryDominant { get => secondaryDominant; }
        public Function ActualResolution { get => actualResolution; }
        public Function ExpectedResolution { get => expectedResolution; }

        private readonly Function secondaryDominant;
        private readonly Function actualResolution;
        private readonly Function expectedResolution;

        public Ellipse(Function secondaryDominant, Function expectedResolution, Function actualResolution)
        {
            this.secondaryDominant = secondaryDominant;
            this.actualResolution = actualResolution;
            this.expectedResolution = expectedResolution;
        }
    }
}
