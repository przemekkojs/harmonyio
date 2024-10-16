using Algorithm.Algorithm.Rules;
using Algorithm.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Algorithm.Mistakes
{
    public class Mistake
    {
        public Rule BrokenRule { get => brokenRule; }
        public List<Stack> Stacks { get => stacks; }

        private readonly Rule brokenRule;
        private readonly List<Stack> stacks;

        public Mistake(Rule brokenRule)
        {
            this.stacks = [];
            this.brokenRule = brokenRule;
        }

        public void AddStack(Stack toAdd)
        {
            if (!stacks.Contains(toAdd))
                stacks.Add(toAdd);
        }
    }
}
