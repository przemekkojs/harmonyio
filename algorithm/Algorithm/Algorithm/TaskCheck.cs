using Algorithm.Algorithm.Mistakes;
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

        public List<Mistake> Check()
        {
            List<Function> userFunctions = userSolution
                .Select(x => x.BaseChord)
                .ToList();

            List<Function> taskFunctions = task.Bars
                .Select(x => x.Functions)
                .SelectMany(x => x)
                .ToList();

            if (!AreTasksEqual(userFunctions, taskFunctions))
                throw new Exception("Something's wrong with a task");

            return GetAllMistakes();
        }

        private static bool AreTasksEqual(List<Function> userFunctions, List<Function> taskFunctions) => userFunctions.SequenceEqual(taskFunctions);

        private List<Mistake> GetAllMistakes()
        {
            List<Mistake> mistakes = [];

            foreach (Rule rule in activeRules)
            {
                Mistake toAdd = new(rule);

                for (int index = 0; index < userSolution.Count; index++)
                {
                    switch (rule.ExpectedParametersCount)
                    {
                        case 1:
                            Stack toCheck = userSolution[index];

                            if (!rule.IsSatisfied(toCheck))
                                toAdd.AddStack(toCheck);

                            break;
                        case 2:
                            if (index == userSolution.Count - 1)
                                break;

                            Stack toCheck1 = userSolution[index];
                            Stack toCheck2 = userSolution[index];

                            if (!rule.IsSatisfied(toCheck1, toCheck2))
                                toAdd.AddStack(toCheck1);

                            break;
                        default:
                            throw new Exception($"Something is wrong with a \"{rule.Name}\" rule.");
                    }
                }
            }

            return mistakes;
        }
    }
}
