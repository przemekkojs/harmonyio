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

        public abstract bool IsSatisfied(params Stack[] functions);

        protected bool ValidateParametersCount(params Stack[] functions)
        {
            if (OneFunction && functions.Length != 1)
                return false;
            else if (!OneFunction && functions.Length != 2)
                return false;
            
            return true;
        }
    }
}