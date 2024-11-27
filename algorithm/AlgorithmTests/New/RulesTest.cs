using Algorithm.New;

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
