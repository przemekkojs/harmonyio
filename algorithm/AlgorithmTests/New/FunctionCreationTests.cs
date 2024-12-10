﻿using Algorithm.New.Music;
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

            Assert.True(testTrue);
            Assert.False(testFalse);
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
    }
}
