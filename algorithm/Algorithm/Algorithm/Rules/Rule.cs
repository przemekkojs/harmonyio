using Algorithm.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Algorithm.Rules
{
    public abstract class Rule
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        protected Rule(string name, string description="")
        {
            this.Name = name;
            this.Description = description;
        }

        public abstract bool IsSatisfied(params Function[] functions);
    }
}
