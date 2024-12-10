using Algorithm.New.Music;
using Index = Algorithm.New.Music.Index;

namespace AlgorithmTests.New
{
    public class FunctionCreationTests
    {
        [Fact]
        public void TTtest()
        {
            var tonation = Tonation.CMajor;

            Function function = new(
                minor: false,
                symbol: Symbol.T,
                tonation: tonation,
                index: new Index()
            );
        }

        [Fact]
        public void SiiTtest()
        {

        }

        [Fact]
        public void TiiiTtest()
        {

        }

        [Fact]
        public void DiiiTtest()
        {

        }

        [Fact]
        public void STtest()
        {

        }

        [Fact]
        public void DTtest()
        {

        }

        [Fact]
        public void TviTtest()
        {

        }

        [Fact]
        public void SviTtest()
        {

        }

        [Fact]
        public void DviiTtest()
        {

        }

        [Fact]
        public void SviiTtest()
        {

        }
    }
}
