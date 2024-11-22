using Algorithm.New.Algorithm.Parsers.SolutionParser;
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
            var functions = parsedProblem.Functions;
            var minor = parsedProblem.Minor != 1;

            // TODO: Poprawić tonację (dodać parametr minor itd.)
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

        public static string ParseProblemToString(Problem problem)
        {
            return "";
        }
    }
}
