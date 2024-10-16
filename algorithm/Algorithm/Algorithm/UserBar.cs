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
        public int LastEmptyStack { get; private set; }
        public int Length { get => userStacks.Count; }

        private readonly List<UserStack> userStacks;

        public UserBar()
        {
            this.userStacks = [];
            LastEmptyStack = 1;
        }

        public void AddStack(UserStack userStack)
        {            
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
