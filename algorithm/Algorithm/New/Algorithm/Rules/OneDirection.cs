using Algorithm.New.Music;
using Algorithm.New.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.New.Algorithm.Rules
{
    internal class OneDirection : Rule
    {
        public OneDirection() : base(
            name: "Ruch jednokierunkowy",
            description: "Czy w ramach dwóch funkcji, wszystkie głosy wykonały ruch w jednym kierunku?") { }

        public override bool IsSatisfied(params Stack[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var stack1 = functions[0];
            var stack2 = functions[1];

            int length = stack1.Notes.Count;

            if (stack1.Notes.Count != stack2.Notes.Count)
                length = Math.Min(stack1.Notes.Count, stack2.Notes.Count);

            if (length == 0)
                return true;

            bool currentUp = Interval.IsLower(stack1.Notes[0], stack1.Notes[0]);
            bool lastUp = currentUp;

            for (int index = 1; index < length; index++)
            {
                currentUp = Interval.IsLower(stack1.Notes[index], stack1.Notes[index]);

                if (currentUp != lastUp)
                    return true;
            }

            // If all directions are the same, we are going in the same direction.
            return false;
        }
    }
}
