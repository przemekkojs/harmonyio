﻿using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules
{
    public abstract class Rule
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool OneFunction { get; protected set; }

        protected Rule(string name, string description, bool oneFunction=false)
        {
            Name = name;
            Description = description;
            OneFunction = oneFunction;
        }

        public abstract bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks);

        protected bool ValidateParametersCount(params Stack[] stacks)
        {
            if (OneFunction && stacks.Length != 1)
                return false;
            else if (!OneFunction && stacks.Length != 2)
                return false;
            
            return true;
        }
    }
}
