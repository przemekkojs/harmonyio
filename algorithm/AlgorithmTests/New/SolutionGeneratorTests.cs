using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Generators;
using Algorithm.New.Algorithm.Parsers.ProblemParser;
using Algorithm.New.Music;

namespace AlgorithmTests.New
{
    public class SolutionGeneratorTests
    {
        [Fact]
        public void Test4Functions()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_4_FUNCTIONS);
            var solution = SolutionGenerator.GenerateLinear(problem);

            Assert.True(solution.Stacks.Count == 4);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);

            Assert.True(mistakes != null);
            Assert.True(mistakes.Count == 0);
        }

        [Fact]
        public void Test8FunctionsEasy()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_8_FUNCTIONS);
            var solution = SolutionGenerator.GenerateLinear(problem);

            Assert.True(solution.Stacks.Count == 8);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);
            
            Assert.True(mistakes != null);
            Assert.True(mistakes.Count == 0);
        }

        [Fact]
        public void Test8Functions()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_8_FUNCTIONS_2);
            var solution = SolutionGenerator.GenerateLinear(problem);

            Assert.True(solution.Stacks.Count == 8);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);
                        
            Assert.True(mistakes != null);
            Assert.True(mistakes.Count == 0);
        }

        [Fact]
        public void Test16FunctionsEasy()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_16_FUNCTIONS);
            var solution = SolutionGenerator.GenerateLinear(problem);

            Assert.True(solution.Stacks.Count == 16);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);
            
            Assert.True(mistakes != null);
            Assert.True(mistakes.Count == 0);
        }

        [Fact]
        public void Test16FunctionsReal()
        {
            var tonation = Tonation.CMajor;
            var metre = Metre.Meter2_4;

            List<Function> functions =
            [
                new Function (
                    new Algorithm.New.Music.Index(0, 0, 4),
                    Symbol.T,
                    false,
                    tonation,
                    root: Component.Root
                ),
                new Function (
                    new Algorithm.New.Music.Index(0, 1, 4),
                    Symbol.S,
                    false,
                    tonation,
                    position: Component.Root
                ),
                new Function (
                    new Algorithm.New.Music.Index(1, 0, 4),
                    Symbol.D,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(1, 1, 4),
                    Symbol.Tvi,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(2, 0, 4),
                    Symbol.Sii,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(2, 1, 4),
                    Symbol.D,
                    false,
                    tonation,
                    root: Component.Fifth
                ),
                new Function (
                    new Algorithm.New.Music.Index(3, 0, 4),
                    Symbol.T,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(3, 1, 4),
                    Symbol.Dvii,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(4, 0, 4),
                    Symbol.Tiii,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(4, 1, 4),
                    Symbol.Svi,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(5, 0, 4),
                    Symbol.Sii,
                    false,
                    tonation,
                    added: [Component.Seventh]
                ),
                new Function (
                    new Algorithm.New.Music.Index(5, 1, 4),
                    Symbol.D,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(6, 0, 4),
                    Symbol.Tvi,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(6, 1, 4),
                    Symbol.S,
                    false,
                    tonation
                ),
                new Function (
                    new Algorithm.New.Music.Index(7, 0, 4),
                    Symbol.D,
                    false,
                    tonation,
                    added: [Component.Seventh]
                ),
                new Function (
                    new Algorithm.New.Music.Index(7, 1, 4),
                    Symbol.T,
                    false,
                    tonation
                ),
            ];

            var problem = new Problem(functions, metre, tonation);
            var solution = SolutionGenerator.GenerateLinear(problem);

            Assert.True(solution.Stacks.Count > 0);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);
            
            Assert.True(mistakes != null);
            Assert.True(mistakes.Count == 0);
        }
    }
}
