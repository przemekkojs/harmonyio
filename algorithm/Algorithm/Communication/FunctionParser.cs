using Algorithm.Music;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Function = Algorithm.Music.Function;

namespace Algorithm.Communication
{
    public class ParsedFunction
    {
        public bool IsMain { get; set; }
        public bool Minor { get; set; }        
        public string Root { get; set; }
        public string Position { get; set; }
        public string Symbol { get; set; }        
        public string Removed { get; set; }
        public List<string> Alterations { get; set; }
        public List<string> Added { get; set; }

        public int BarIndex { get; set; }
        public int CorrespondingStackIndex { get; set; }

        public ParsedFunction(Function function)
        {
            Symbol = function.Symbol.FunctionSymbol.ToString();
            Removed = function.Symbol.Removed[0].Symbol.ToString();
            Minor = function.Symbol.Minor;
            IsMain = function.IsMain;

            if (function.Symbol.Root != null)
                Root = function.Symbol.Root.Symbol.ToString();
            else
                Root = "";

            if (function.Symbol.Position != null)
                Position = function.Symbol.Position.Symbol.ToString();
            else
                Position = "";

            Added = [];
            Alterations = [];

            GetAdded(function);
            GetAlterations(function);
        }

        private void GetAdded(Function function)
        {
            foreach (var added in function.Symbol.Added)
            {
                Added.Add(added.Symbol.ToString());
            }
        }

        private void GetAlterations(Function function)
        {
            foreach (var alteration in function.Symbol.Alterations)
            {
                var component = alteration.ChordComponent.Symbol.ToString();
                var alterationType = alteration.AlterationSymbol.ToString();
                var toAdd = component + alterationType;

                Alterations.Add(toAdd);
            }
        }
    }

    public static class FunctionParser
    {
        public static string ParseFunctionToJson(Function function)
        {
            ParsedFunction parsedFunction = new(function);
            string parsed = JsonConvert.SerializeObject(parsedFunction);
            return parsed;
        }

        public static Function ParseJsonToFunction(string jsonString)
        {
            object? parsedObject = JsonConvert.DeserializeObject(jsonString) ?? throw new ArgumentException("Invalid object");
            ParsedFunction parsed = parsedObject as ParsedFunction ?? throw new InvalidCastException("Invalid JSON.");

            //TODO
            Symbol functionSymbol = null;
            
            Function result = new(
                functionSymbol,
                true
            );

            return result;
        }
    }
}
