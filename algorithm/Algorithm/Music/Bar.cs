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
        public List<Function> Functions { get => functions; }

        private readonly Tonation tonation;
        private readonly Meter meter;
        private readonly List<Function> functions;

        public Bar(Tonation tonation, Meter meter)
        {
            this.tonation = tonation;
            this.meter = meter;

            this.functions = new List<Function>();
        }

        public bool AddFunction(Function function)
        {
            int length = functions.Sum(x => x.Duration);
            int lengthAfter = length + function.Duration;

            if (lengthAfter > meter.Count * meter.Value)
                return false;

            functions.Add(function);

            return true;
        }

        public void AddFunctionsRange(List<Function> functions)
        {
            foreach(Function function in functions)
            {
                AddFunction(function);
            }
        }
    }
}
