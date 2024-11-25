using Algorithm.New.Music;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTests.New
{
    public class FunctionParsingTests
    {
        [Fact]
        public void ParsingWorks()
        {
            var functionJson = "{\"minor\":false,\"symbol\":\"T\",\"position\":\"\",\"root\":\"\",\"removed\":\"\",\"alterations\":[],\"added\":[],\"barIndex\":0,\"verticalIndex\":0}";

            var parseFunction = JsonConvert
                .DeserializeObject<ParsedFunction>(functionJson);

            if (parseFunction == null)
                Assert.Fail();

            Assert.True(true);
        }
    }
}
