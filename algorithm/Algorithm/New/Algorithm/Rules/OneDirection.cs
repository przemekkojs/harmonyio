using Algorithm.New.Music;
using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
        }
    }
}
