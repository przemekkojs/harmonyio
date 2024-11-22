using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Music;

namespace AlgorithmTests.New
{
    public class ProblemGenerationTests
    {
        [Fact]
        public void ShortGeneration()
        {
            var barsCount = 2;
            var metre = Metre.Meter2_4;
            var tonation = Tonation.CMajor;

            var generation = Algorithm.New.Algorithm.Generators.ProblemGenerator
                .Generate(barsCount, metre, tonation);

            var problem = new Problem(generation, metre, tonation);

            Assert.True(ProblemChecker.CheckProblem(problem).Count == 0, "This should be true");
        }
    }
}
