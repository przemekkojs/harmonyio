using Algorithm.New.Music;
using Newtonsoft.Json;

namespace Algorithm.New.Algorithm.Parsers.SolutionParser
{
    public static class Parser
    {
        public static Solution ParseJsonToSolution(string jsonString, Problem problem)
        {
            JsonSolution? parsedTask = JsonConvert.DeserializeObject<JsonSolution>(jsonString) ?? throw new ArgumentException("Invalid JSON string.");

            var tonation = Tonation.GetTonation(parsedTask.SharpsCount, parsedTask.FlatsCount);
            var metre = Metre.GetMetre(parsedTask.MeterValue, parsedTask.MeterCount);


            return null;
        }

        public static string ParseSolutionToJson(Solution solution)
        {
            return "";
        }
    }
}
