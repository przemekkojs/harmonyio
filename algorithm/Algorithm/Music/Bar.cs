using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Music
{
    public class Bar
    {
        public Tonation Tonation { get => tonation; }
        public Meter Meter { get => meter; }

        private readonly Tonation tonation;
        private readonly Meter meter;

        public Bar(Tonation tonation, Meter meter)
        {
            this.tonation = tonation;
            this.meter = meter;
        }
    }
}
