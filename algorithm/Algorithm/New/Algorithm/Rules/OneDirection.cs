﻿using Algorithm.New.Music;
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

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            var stack1 = stacks[0];
            var stack2 = stacks[1];

            int length = stack1.Notes.Count;

            if (stack1.Notes.Count != stack2.Notes.Count)
                length = Math.Min(stack1.Notes.Count, stack2.Notes.Count);

            if (length == 0)
                return true;

            bool currentUp = Interval.IsLower(stack1.Notes[0], stack1.Notes[0]);
            bool lastUp = currentUp;

            for (int index = 1; index < length; index++)
            {
                var note1 = stack1.Notes[index];
                var note2 = stack2.Notes[index];

                if (note1 == null || note2 == null)
                    continue;

                currentUp = Interval.IsLower(note1, note2);

                if (currentUp != lastUp)
                    return true;
            }

            // If all directions are the same, we are going in the same direction.
            return false;
        }
    }
}
