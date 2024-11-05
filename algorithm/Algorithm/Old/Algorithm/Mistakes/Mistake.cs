using Algorithm.Old.Music;
using Algorithm.Old.Algorithm;
using Algorithm.Old.Algorithm.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Old.Algorithm.Mistakes
{
    public class Mistake
    {
        public Rule BrokenRule { get => brokenRule; }
        public List<Stack> Stacks { get => stacks; }

        private readonly Rule brokenRule;
        private readonly List<Stack> stacks;

        public Mistake(Rule brokenRule)
        {
            stacks = [];
            this.brokenRule = brokenRule;
        }

        public void AddStack(Stack toAdd)
        {
            if (!stacks.Contains(toAdd))
                stacks.Add(toAdd);
        }
    }
}
