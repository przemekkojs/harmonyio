﻿using Algorithm.New;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Generators;
using Algorithm.New.Algorithm.Parsers.ProblemParser;

namespace AlgorithmTests.New
{
    public class SolutionGeneratorTests
    {
        [Fact]
        public void Test4Functions()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_4_FUNCTIONS);
            var solution = SolutionGenerator.GenerateLinear(problem);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);

            Assert.True(solution.Stacks.Count == 4);
            Assert.True(mistakes.Count == 0);
        }

        [Fact]
        public void Test8FunctionsEasy()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_8_FUNCTIONS);
            var solution = SolutionGenerator.GenerateLinear(problem);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);

            Assert.True(solution.Stacks.Count == 8);
            Assert.True(mistakes.Count == 0);
        }

        [Fact]
        public void Test8Functions()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_8_FUNCTIONS_2);
            var solution = SolutionGenerator.GenerateLinear(problem);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);

            Assert.True(solution.Stacks.Count == 8);
            Assert.True(mistakes.Count == 0);
        }

        [Fact]
        public void Test16FunctionsEasy()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_16_FUNCTIONS);
            var solution = SolutionGenerator.GenerateLinear(problem);

            var mistakes = SolutionChecker.CheckSolution(solution, Constants.Settings);

            Assert.True(solution.Stacks.Count == 16);
            Assert.True(mistakes.Count == 0);
        }
    }
}
