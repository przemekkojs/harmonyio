using Algorithm.New.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public class RootEqualsPosition : Rule
    {
        public RootEqualsPosition() : base(
            id: 212,
            name: "Oparcie i pozycja ze składnikami dodanymi",
            description: "Oparcie i pozycja nie mogą być takie same przy funkcjach z dodanymi składnikami dysonującymi i żadnym usuniętym",
            oneFunction: true)
        { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (!ValidateParametersCount(functions))
                return false;

            var function = functions[0];

            var position = function.Position;
            var root = function.Root;
            var removed = function.Removed;
            var added = function.Added;

            if (position != null && root != null)
            {
                var addedCount = added.Count;

                if (addedCount > 1 && removed != null)
                    return false;
            }

            return true;
        }
    }
}
