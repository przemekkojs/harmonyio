using Algorithm.New.Utils;

namespace AlgorithmTests.New
{
    public class RhytmizerTests
    {
        [Fact]
        public void Test2_4()
        {
            var metreCount = 2;
            var metreValue = 4;            

            var test1 = Rhytm.GetRhytmicScheme(functionsInBar: 1, metreCount, metreValue);
            var test2 = Rhytm.GetRhytmicScheme(functionsInBar: 2, metreCount, metreValue);
            var test3 = Rhytm.GetRhytmicScheme(functionsInBar: 3, metreCount, metreValue);
            var test4 = Rhytm.GetRhytmicScheme(functionsInBar: 4, metreCount, metreValue);
            var test5 = Rhytm.GetRhytmicScheme(functionsInBar: 5, metreCount, metreValue);
            var test6 = Rhytm.GetRhytmicScheme(functionsInBar: 6, metreCount, metreValue);
            var test7 = Rhytm.GetRhytmicScheme(functionsInBar: 7, metreCount, metreValue);
            var test8 = Rhytm.GetRhytmicScheme(functionsInBar: 8, metreCount, metreValue);

            List<int> actual1 = [8];
            List<int> actual2 = [4, 4];
            List<int> actual3 = [4, 2, 2];
            List<int> actual4 = [2, 2, 2, 2];
            List<int> actual5 = [2, 2, 2, 1, 1];
            List<int> actual6 = [2, 2, 1, 1, 1, 1];
            List<int> actual7 = [2, 1, 1, 1, 1, 1, 1];
            List<int> actual8 = [1, 1, 1, 1, 1, 1, 1, 1];

            Assert.True(test1.SequenceEqual(actual1));
            Assert.True(test2.SequenceEqual(actual2));
            Assert.True(test3.SequenceEqual(actual3));
            Assert.True(test4.SequenceEqual(actual4));
            Assert.True(test5.SequenceEqual(actual5));
            Assert.True(test6.SequenceEqual(actual6));
            Assert.True(test7.SequenceEqual(actual7));
            Assert.True(test8.SequenceEqual(actual8));
        }

        [Fact]
        public void Test3_4()
        {
            var metreCount = 3;
            var metreValue = 4;

            var test1 = Rhytm.GetRhytmicScheme(functionsInBar: 1, metreCount, metreValue);
            var test2 = Rhytm.GetRhytmicScheme(functionsInBar: 2, metreCount, metreValue);
            var test3 = Rhytm.GetRhytmicScheme(functionsInBar: 3, metreCount, metreValue);
            var test4 = Rhytm.GetRhytmicScheme(functionsInBar: 4, metreCount, metreValue);
            var test5 = Rhytm.GetRhytmicScheme(functionsInBar: 5, metreCount, metreValue);
            var test6 = Rhytm.GetRhytmicScheme(functionsInBar: 6, metreCount, metreValue);
            var test7 = Rhytm.GetRhytmicScheme(functionsInBar: 7, metreCount, metreValue);
            var test8 = Rhytm.GetRhytmicScheme(functionsInBar: 8, metreCount, metreValue);

            List<int> actual1 = [12];
            List<int> actual2 = [8, 4];
            List<int> actual3 = [4, 4, 4];
            List<int> actual4 = [4, 4, 2, 2];
            List<int> actual5 = [4, 2, 2, 2, 2];
            List<int> actual6 = [2, 2, 2, 2, 2, 2];
            List<int> actual7 = [2, 2, 2, 2, 2, 1, 1];
            List<int> actual8 = [2, 2, 2, 2, 1, 1, 1, 1];

            Assert.True(test1.SequenceEqual(actual1));
            Assert.True(test2.SequenceEqual(actual2));
            Assert.True(test3.SequenceEqual(actual3));
            Assert.True(test4.SequenceEqual(actual4));
            Assert.True(test5.SequenceEqual(actual5));
            Assert.True(test6.SequenceEqual(actual6));
            Assert.True(test7.SequenceEqual(actual7));
            Assert.True(test8.SequenceEqual(actual8));
        }

        [Fact]
        public void Test4_4()
        {
            var metreCount = 4;
            var metreValue = 4;

            var test1 = Rhytm.GetRhytmicScheme(functionsInBar: 1, metreCount, metreValue);
            var test2 = Rhytm.GetRhytmicScheme(functionsInBar: 2, metreCount, metreValue);
            var test3 = Rhytm.GetRhytmicScheme(functionsInBar: 3, metreCount, metreValue);
            var test4 = Rhytm.GetRhytmicScheme(functionsInBar: 4, metreCount, metreValue);
            var test5 = Rhytm.GetRhytmicScheme(functionsInBar: 5, metreCount, metreValue);
            var test6 = Rhytm.GetRhytmicScheme(functionsInBar: 6, metreCount, metreValue);
            var test7 = Rhytm.GetRhytmicScheme(functionsInBar: 7, metreCount, metreValue);
            var test8 = Rhytm.GetRhytmicScheme(functionsInBar: 8, metreCount, metreValue);

            List<int> actual1 = [16];
            List<int> actual2 = [8, 8];
            List<int> actual3 = [8, 4, 4];
            List<int> actual4 = [4, 4, 4, 4];
            List<int> actual5 = [4, 4, 4, 2, 2];
            List<int> actual6 = [4, 4, 2, 2, 2, 2];
            List<int> actual7 = [4, 2, 2, 2, 2, 2, 2];
            List<int> actual8 = [2, 2, 2, 2, 2, 2, 2, 2];

            Assert.True(test1.SequenceEqual(actual1));
            Assert.True(test2.SequenceEqual(actual2));
            Assert.True(test3.SequenceEqual(actual3));
            Assert.True(test4.SequenceEqual(actual4));
            Assert.True(test5.SequenceEqual(actual5));
            Assert.True(test6.SequenceEqual(actual6));
            Assert.True(test7.SequenceEqual(actual7));
            Assert.True(test8.SequenceEqual(actual8));
        }

        [Fact]
        public void Test6_8()
        {
            var metreCount = 6;
            var metreValue = 8;

            var test1 = Rhytm.GetRhytmicScheme(functionsInBar: 1, metreCount, metreValue);
            var test2 = Rhytm.GetRhytmicScheme(functionsInBar: 2, metreCount, metreValue);
            var test3 = Rhytm.GetRhytmicScheme(functionsInBar: 3, metreCount, metreValue);
            var test4 = Rhytm.GetRhytmicScheme(functionsInBar: 4, metreCount, metreValue);
            var test5 = Rhytm.GetRhytmicScheme(functionsInBar: 5, metreCount, metreValue);
            var test6 = Rhytm.GetRhytmicScheme(functionsInBar: 6, metreCount, metreValue);
            var test7 = Rhytm.GetRhytmicScheme(functionsInBar: 7, metreCount, metreValue);
            var test8 = Rhytm.GetRhytmicScheme(functionsInBar: 8, metreCount, metreValue);

            List<int> actual1 = [12];
            List<int> actual2 = [6, 6];
            List<int> actual3 = [6, 4, 2];
            List<int> actual4 = [4, 2, 4, 2];
            List<int> actual5 = [4, 2, 2, 2, 2];
            List<int> actual6 = [2, 2, 2, 2, 2, 2];
            List<int> actual7 = [2, 2, 2, 2, 2, 1, 1];
            List<int> actual8 = [2, 2, 2, 2, 1, 1, 1, 1];

            Assert.True(test1.SequenceEqual(actual1));
            Assert.True(test2.SequenceEqual(actual2));
            Assert.True(test3.SequenceEqual(actual3));
            Assert.True(test4.SequenceEqual(actual4));
            Assert.True(test5.SequenceEqual(actual5));
            Assert.True(test6.SequenceEqual(actual6));
            Assert.True(test7.SequenceEqual(actual7));
            Assert.True(test8.SequenceEqual(actual8));
        }

        [Fact]
        public void Test3_8()
        {
            var metreCount = 3;
            var metreValue = 8;

            var test1 = Rhytm.GetRhytmicScheme(functionsInBar: 1, metreCount, metreValue);
            var test2 = Rhytm.GetRhytmicScheme(functionsInBar: 2, metreCount, metreValue);
            var test3 = Rhytm.GetRhytmicScheme(functionsInBar: 3, metreCount, metreValue);
            var test4 = Rhytm.GetRhytmicScheme(functionsInBar: 4, metreCount, metreValue);
            var test5 = Rhytm.GetRhytmicScheme(functionsInBar: 5, metreCount, metreValue);
            var test6 = Rhytm.GetRhytmicScheme(functionsInBar: 6, metreCount, metreValue);

            List<int> actual1 = [6];
            List<int> actual2 = [4, 2];
            List<int> actual3 = [2, 2, 2];
            List<int> actual4 = [2, 2, 1, 1];
            List<int> actual5 = [2, 1, 1, 1, 1];
            List<int> actual6 = [1, 1, 1, 1, 1, 1];

            Assert.True(test1.SequenceEqual(actual1));
            Assert.True(test2.SequenceEqual(actual2));
            Assert.True(test3.SequenceEqual(actual3));
            Assert.True(test4.SequenceEqual(actual4));
            Assert.True(test5.SequenceEqual(actual5));
            Assert.True(test6.SequenceEqual(actual6));
        }

        [Fact]
        public void TestThrowing()
        {
            Assert.Throws<ArgumentException>(() => Rhytm.GetRhytmicScheme(9, 2, 4));
            Assert.Throws<ArgumentException>(() => Rhytm.GetRhytmicScheme(7, 3, 8));
        }
    }
}
