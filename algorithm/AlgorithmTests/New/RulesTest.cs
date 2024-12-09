using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Rules.Solution;
using Algorithm.New.Music;

namespace AlgorithmTests.New
{
    public class RulesTest
    {
        [Fact]
        public void UniqueIdsTest()
        {
            var ids = Constants.Settings.ActiveRules
                .Select(r => r.Id);

            var uniqueIds = ids
                .Distinct();

            Assert.True(uniqueIds.Count() == ids.Count(), "This should be true");
        }

        [Fact]
        public void AugmentedIntervalTest()
        {
            var settings = new Settings([new AugmentedInterval()]);
            var tonation = Tonation.CMajor;
            var metre = Metre.Meter2_4;

            var stack1 = new Stack(new Algorithm.New.Music.Index()
            {
                Bar = 0,
                Position = 0,
                Duration = 4
            },
                ["C", "F", "Ab", "C"]
            );

            var stack2 = new Stack(new Algorithm.New.Music.Index()
            {
                Bar = 0,
                Position = 1,
                Duration = 4
            },
                ["D", "G", "B", "F"]
            );

            var function1 = new Function(new Algorithm.New.Music.Index()
            {
                Bar = 0,
                Position = 0,
                Duration = 4
            },
                Symbol.S,
                true,
                tonation
            );

            var function2 = new Function(new Algorithm.New.Music.Index()
            {
                Bar = 0,
                Position = 1,
                Duration = 4
            },
                Symbol.D,
                false,
                tonation,
                added: [Component.Seventh]
            );

            var problem = new Problem([function1, function2], metre, tonation);
            var solution = new Solution(problem, [stack1, stack2]);
            var mistakes = SolutionChecker.CheckSolution(solution, settings);

            Assert.True(mistakes != null);
            Assert.True(mistakes.Count == 1);

            // Sprawdzenie, czy działa w 2 strony
            var problem2 = new Problem([function2, function1], metre, tonation);
            var solution2 = new Solution(problem2, [stack2, stack1]);
            var mistakes2 = SolutionChecker.CheckSolution(solution2, settings);

            Assert.True(mistakes2 != null);
            Assert.True(mistakes2.Count == 1);
        }

        [Fact]
        public void DominantSixthResolutionTest()
        {

        }

        [Fact]
        public void SixthResolutionTest()
        {

        }
    }
}
