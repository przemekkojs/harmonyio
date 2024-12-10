using Algorithm.New.Music;
using Newtonsoft.Json;

namespace Algorithm.New.Algorithm.Parsers.ProblemParser
{
    // TODO: Całość
    public static class Parser
    {
        public static Problem ParseJsonToProblem(string jsonString)
        {
            JsonProblem? parsedProblem = JsonConvert
                .DeserializeObject<JsonProblem>(jsonString) ??
                throw new ArgumentException("Invalid JSON string.");

            var metreValue = parsedProblem.MetreValue;
            var metreCount = parsedProblem.MetreCount;
            var flatsCount = parsedProblem.FlatsCount;
            var sharpsCount = parsedProblem.SharpsCount;
            var functions = parsedProblem.Task;
            var minor = parsedProblem.Minor != 1;

            var tonationList = Tonation.GetTonation(sharpsCount, flatsCount);

            var tonation = (minor ?
                tonationList.FirstOrDefault(x => x.Mode == Mode.Minor) :
                tonationList.FirstOrDefault(x => x.Mode == Mode.Major)) ??
                throw new ArgumentException("Invalid tonation");

            var metre = Metre.GetMetre(metreCount, metreValue);

            List<Function> realFuntions = [];

            foreach (var f in functions)
            {
                var toAdd = new Function(f)
                {
                    Tonation = tonation
                };

                realFuntions.Add(toAdd);
            }

            var result = new Problem(realFuntions, metre, tonation);
            return result;
        }

        // TODO: To musi zwracać listę ParsedFunction jako JSON-string
        public static string ParseProblemFunctionsToString(Problem problem)
        {
            List<ParsedFunction> resultList = [];

            foreach (var function in problem.Functions)
            {
                var toAppend = ParsedFunction.CreateFromFunction(function);
                resultList.Add(toAppend);
            }

            var converted = JsonConvert.SerializeObject(resultList);
            return converted;
        }
    }
}
