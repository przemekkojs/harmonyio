using Algorithm.New;

namespace AlgorithmTests.New
{
    public class ProblemRulesTest
    {
        [Fact]
        public void UniqueIds()
        {
            var rules = Constants.ProblemSettings;

            var allIds = rules.Select(x => x.Id);
            var uniqueIds = allIds.Distinct();

            Assert.Equal(allIds.Count(), uniqueIds.Count());
        }
    }
}
