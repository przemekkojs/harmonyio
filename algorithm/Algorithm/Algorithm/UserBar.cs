using Algorithm.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Algorithm
{
    public class UserBar
    {
        public List<UserStack> UserStacks { get => userStacks; }
        public Bar BaseBar { get => baseBar; }
        public int LastEmptyStack { get; private set; }

        private readonly Bar baseBar;
        private readonly List<UserStack> userStacks;

        public UserBar(Bar baseBar)
        {
            this.baseBar = baseBar;
            this.userStacks = [];

            LastEmptyStack = 1;
        }

        public void AddStack(UserStack userStack)
        {
            if (LastEmptyStack >= baseBar.Functions.Count)
                return;

            userStacks.Add(userStack);
            LastEmptyStack++;
        }

        public void RemoveStack(UserStack userStack)
        {
            userStacks.Remove(userStack);
            LastEmptyStack--;
        }
    }
}
