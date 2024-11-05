namespace Algorithm.Old.Algorithm
{
    public class UserBar
    {
        public List<UserStack> UserStacks { get => userStacks; }
        public int LastEmptyStack { get; private set; }
        public int Length { get => userStacks.Count; }

        private readonly List<UserStack> userStacks;

        public UserBar()
        {
            userStacks = [];
            LastEmptyStack = 0;
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
