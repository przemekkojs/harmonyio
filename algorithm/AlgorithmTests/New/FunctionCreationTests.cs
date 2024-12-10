using Algorithm.New.Music;
using Index = Algorithm.New.Music.Index;

namespace AlgorithmTests.New
{
    public class FunctionCreationTests
    {
        private static bool TestSetsEqual(List<List<Component>> test, List<List<Component>> source)
        {
            if (test.Count != source.Count)
                return false;

            var testFlattened = test
                .SelectMany(x => x)
                .OrderBy(x => x.ToString())
                .ToList();

            var sourceFlattened = source
                .SelectMany(x => x)
                .OrderBy(x => x.ToString())
                .ToList();

            return testFlattened.SequenceEqual(sourceFlattened);
        }

        private static readonly Tonation tonation = Tonation.CMajor;

        [Fact]
        public void TestTestingFunction()
        {
            var test1 = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Fifth, Component.Third, Component.Fifth }
            };

            var test2 = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Fifth, Component.Third, Component.Fifth }
            };

            var test3 = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var testTrue = TestSetsEqual(test1, test2);
            var testFalse = TestSetsEqual(test1, test3);

            Assert.True(testTrue, "This must pass for all other tests to work");
            Assert.False(testFalse, "This must pass for all other tests to work");
        }

        [Fact]
        public void TTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Fifth, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);
            
            Assert.True(setsEqual);
        }

        [Fact]
        public void SiiTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Fifth, Component.Third, Component.Third }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Sii,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TiiiTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void DiiiTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Diii,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void STtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Fifth, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.S,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void DTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Fifth, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.D,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TviTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tvi,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void SviTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Svi,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void DviiTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Dvii,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void SviiTtest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Svii,
                tonation: tonation,
                index: new Index()
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void T7Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Seventh, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                added: [Component.Seventh]
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void Tiii7Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Seventh, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Seventh]
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void T6Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Sixth, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                added: [Component.Sixth]
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void Tiii6Ttest()
        {
            Assert.Throws<ArgumentException>(() => new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Sixth]
            ));
        }

        [Fact]
        public void T9Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Ninth, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                added: [Component.Ninth]
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void Tiii9Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Ninth, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Ninth]
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void T9No1Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Fifth, Component.Ninth, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                added: [Component.Ninth],
                removed: Component.Root
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void T9No5Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Ninth, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                added: [Component.Ninth],
                removed: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void Tiii9No1Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Fifth, Component.Ninth, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Ninth],
                removed: Component.Root
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void Tiii9No5Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Ninth, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Ninth],
                removed: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void T7No5Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Seventh },
                new () { Component.Root, Component.Seventh, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                added: [Component.Seventh],
                removed: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void Tiii7No5Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Seventh },
                new () { Component.Root, Component.Seventh, Component.Third, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Seventh],
                removed: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void D7add6Ttest()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Third, Component.Sixth, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.D,
                tonation: tonation,
                index: new Index(),
                added: [Component.Sixth, Component.Seventh],
                removed: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void Other67Throws()
        {
            Assert.Throws<ArgumentException>(() => new Function(
                minor: false,
                symbol: Symbol.Dvii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Sixth, Component.Seventh],
                removed: Component.Fifth
            ));

            Assert.Throws<ArgumentException>(() => new Function(
                minor: true,
                symbol: Symbol.D,
                tonation: tonation,
                index: new Index(),
                added: [Component.Sixth, Component.Seventh],
                removed: Component.Fifth
            ));

            Assert.Throws<ArgumentException>(() => new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                added: [Component.Sixth, Component.Seventh],
                removed: Component.Fifth
            ));
        }

        [Fact]
        public void TSameRootAndPositionRootGood()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Third, Component.Root, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                position: Component.Root,
                root: Component.Root
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TSameRootAndPositionFifthGood()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Third, Component.Fifth, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                position: Component.Fifth,
                root: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TiiiSameRootAndPositionRootGood()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Third, Component.Root, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                position: Component.Root,
                root: Component.Root
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TiiiSameRootAndPositionThirdGood()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                position: Component.Third,
                root: Component.Third
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TDifferentRootAndPositionGood()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Fifth, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index(),
                position: Component.Root,
                root: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TiiiDifferentRootAndPositionGood()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Root, Component.Third, Component.Fifth },
                new () { Component.Root, Component.Third, Component.Third, Component.Fifth }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                position: Component.Root,
                root: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }

        [Fact]
        public void TSameRootAndPositionSeventhGood()
        {
            var testSet = new List<List<Component>>()
            {
                new () { Component.Root, Component.Third, Component.Seventh, Component.Seventh }
            };

            var function = new Function(
                minor: false,
                symbol: Symbol.Tiii,
                tonation: tonation,
                index: new Index(),
                added: [Component.Seventh],
                position: Component.Seventh,
                root: Component.Seventh,
                removed: Component.Fifth
            );

            var functionPossibleComponents = function.PossibleComponents;
            var setsEqual = TestSetsEqual(testSet, functionPossibleComponents);

            Assert.True(setsEqual);
        }
    }
}
