﻿using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class ValidPosition : Rule
    {
        public ValidPosition() : base(
            id: 114,
            name: "Poprawna pozycja",
            description: "Czy pozycja rozwiązania jest zgodna z pozycją problemu",
            oneFunction: true) { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            return true;
        }
    }
}
