using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Rules.Problem;
using Algorithm.New.Algorithm.Rules.Solution;
using Algorithm.New.Music;

namespace AlgorithmTests.New
{
    public class RulesTest
    {
        [Fact]
        public void AllComponentsSatisfiedTest()
        {
            var settings = new Settings([new AllComponentsSatisfied()]);
            var tonation = Tonation.CMajor;
            var metre = Metre.Meter2_4;

            var stack1 = new Stack(new Algorithm.New.Music.Index()
            {
                Bar = 0,
                Position = 0,
                Duration = 4
            },
                ["C", "F", null, null]
            );

            var stack2 = new Stack(new Algorithm.New.Music.Index()
            {
                Bar = 0,
                Position = 0,
                Duration = 4
            },
                ["C", "C", "C", "C"]
            );

            var function = new Function(new Algorithm.New.Music.Index()
            {
                Bar = 0,
                Position = 0,
                Duration = 4
            },
                Symbol.S,
            true,
            tonation
            );

            var problem = new Problem([function], metre, tonation);
            var solution1 = new Solution(problem, [stack1]);
            var mistakes1 = SolutionChecker.CheckSolution(solution1, settings);

            Assert.True(mistakes1 != null);
            Assert.True(mistakes1.Count == 2); // 2, bo 2 nut brakuje

            var solution2 = new Solution(problem, [stack2]);
            var mistakes2 = SolutionChecker.CheckSolution(solution2, settings);

            Assert.True(mistakes2 != null);
            Assert.True(mistakes2.Count == 1);
            Assert.True(mistakes2[0].MistakeCode == settings.ActiveRules[0].Id);
        }

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

        [Fact]
        public void FunctionAfterSeventhTests()
        {
            var rule = new FunctionAfterSeventh();
            var metre = Metre.Meter2_4;
            var tonation = Tonation.CMajor;

            var T7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.T,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var Sii7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.Sii,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var Tiii7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.Tiii,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var Diii7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.Diii,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var S7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.S,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var D7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.D,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var Tvi7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.Tvi,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var Svi7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.Svi,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var Dvii7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.Dvii,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );

            var Svii7 = new Function(
                index: new Algorithm.New.Music.Index(0, 0, 4),
                symbol: Symbol.Svii,
                minor: false,
                added: [Component.Seventh],
                tonation: tonation
            );


            var T = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.T,
                minor: false,
                tonation: tonation
            );

            var Sii = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.Sii,
                minor: false,
                tonation: tonation
            );

            var Tiii = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.Tiii,
                minor: false,
                tonation: tonation
            );

            var Diii = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.Diii,
                minor: false,
                tonation: tonation
            );

            var S = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.S,
                minor: false,
                tonation: tonation
            );

            var D = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.D,
                minor: false,
                tonation: tonation
            );

            var Tvi = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.Tvi,
                minor: false,
                tonation: tonation
            );

            var Svi = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.Svi,
                minor: false,
                tonation: tonation
            );

            var Dvii = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.Dvii,
                minor: false,
                tonation: tonation
            );

            var Svii = new Function(
                index: new Algorithm.New.Music.Index(0, 1, 4),
                symbol: Symbol.Svii,
                minor: false,
                tonation: tonation
            );

            List<(Function, Function)> pairs =
            [
                (D7, T),
                (D7, Tiii),
                (D7, Tvi),

                (Tvi7, Sii),
                (Tvi7, S),
                (Tvi7, Dvii),

                (Svi7, Sii),
                (Svi7, S),
                (Svi7, Dvii),
                (Svi7, Svii),

                (Dvii7, Tiii),
                (Dvii7, Diii),
                (Dvii7, T),
                (Dvii7, D),

                (Svii7, Tiii),
                (Svii7, Diii),
                (Svii7, T),
                (Svii7, D),

                (T7, S),
                (T7, Sii),
                (T7, Svi),
                (T7, Tvi),

                (Sii7, D),
                (Sii7, Tiii),
                (Sii7, Diii),
                (Sii7, Dvii),
                (Sii7, Svii),

                (Tiii7, Tvi),
                (Tiii7, Svi),
                (Tiii7, T),
                (Tiii7, S),

                (Diii7, Tvi),
                (Diii7, T),

                (S7, Dvii),
                (S7, Svii),
                (S7, D),
                (S7, Sii),
            ];

            foreach (var pair in pairs)
            {
                var function1 = pair.Item1;
                var function2 = pair.Item2;

                var satisfied = rule.IsSatisfied(function1, function2);

                Assert.True(satisfied, $"{function1.Symbol}7 - {function2.Symbol}");
            }
        }
    }
}
