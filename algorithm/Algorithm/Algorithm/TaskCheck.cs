using Algorithm.Algorithm.Rules;
using Algorithm.Music;

namespace Algorithm.Algorithm
{
    public class TaskCheck
    {
        private readonly List<Rules.Rule> activeRules;
        private readonly Music.Task task;
        private readonly List<Stack> userSolution;

        public TaskCheck(Music.Task task, List<Stack> userSolution, List<Rule> activeRules)
        {
            this.task = task;
            this.userSolution = userSolution;
            this.activeRules = activeRules;
        }

        public void Check()
        {
            List<Function> userFunctions = userSolution
                .Select(x => x.BaseChord)
                .ToList();

            List<Function> taskFunctions = task.Bars
                .Select(x => x.Functions)
                .SelectMany(x => x)
                .ToList();

            bool areEqual = userFunctions.SequenceEqual(taskFunctions);

            if (!areEqual)
                throw new Exception("Something's wrong with a task");

            List<Stack> mistaken = [];

            for (int index = 0; index < userFunctions.Count; index++)
            {
                foreach (Rule rule in activeRules)
                {
                    switch (rule.ExpectedParametersCount)
                    {
                        case 1:
                            Stack toCheck = userSolution[index];

                            if (!rule.IsSatisfied(toCheck))
                                mistaken.Add(toCheck);

                            break;
                        case 2:
                            if (index == userFunctions.Count - 1)
                                continue;

                            Stack toCheck1 = userSolution[index];
                            Stack toCheck2 = userSolution[index];

                            if (!rule.IsSatisfied(toCheck1, toCheck2))
                                mistaken.AddRange([toCheck1, toCheck2]);

                            break;
                        default:
                            throw new Exception($"Something is wrong with a \"{rule.Name}\" rule.");
                    }
                }
            }
        }
    }
}
