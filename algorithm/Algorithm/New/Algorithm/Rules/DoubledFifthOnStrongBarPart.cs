﻿using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules
{
    public class DoubledFifthOnStrongBarPart : Rule
    {
        public DoubledFifthOnStrongBarPart() : base(
            name: "Dwojenie kwinty na mocnej części taktu",
            description: "Czy w ramach jednej funkcji, przypadającej na mocnej części taktu, podwojona została kwinta, przy oparciu na kwincie?",
            oneFunction: true) { }

        // TODO: Jakoś trzeba wsm przekazać metrum... XD
        public override bool IsSatisfied(params Stack[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var stack = functions[0];

            int verticalIndex = stack.Index.Position;

            if (verticalIndex == 0)
            {
                // TODO: Add logic here
            }

            return true;
        }
    }
}
