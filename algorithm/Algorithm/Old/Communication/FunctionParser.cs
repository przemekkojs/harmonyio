using Algorithm.Old.Music;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Function = Algorithm.Old.Music.Function;

namespace Algorithm.Old.Communication
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
            Removed = function.Symbol.Removed != null ? function.Symbol.Removed.Symbol.ToString() : "";
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

            Symbol functionSymbol = new(
                minor: parsed.Minor,
                functionSymbol: GetSymbol(parsed.Symbol),
                root: GetFunctionComponent(parsed.Root),
                position: GetFunctionComponent(parsed.Position),
                added: GetManyFunctionComponents(parsed.Added),
                removed: GetFunctionComponent(parsed.Removed),
                suspensions: [], //TODO
                alterations: [] // TODO
            );

            Function result = new(
                functionSymbol,
                true
            );

            return result;
        }

        public static FunctionSymbol GetSymbol(string symbol)
        {
            return symbol.ToLower() switch
            {
                "t" => FunctionSymbol.T,
                "sii" => FunctionSymbol.Sii,
                "tiii" => FunctionSymbol.Tiii,
                "diii" => FunctionSymbol.Diii,
                "s" => FunctionSymbol.S,
                "d" => FunctionSymbol.D,
                "tvi" => FunctionSymbol.Tvi,
                "svi" => FunctionSymbol.Svi,
                "dvii" => FunctionSymbol.Dvii,
                "svii" => FunctionSymbol.Svii,
                _ => throw new ArgumentException("Invalid symbol")
            };
        }

        public static FunctionComponent? GetFunctionComponent(string component)
        {
            return component switch
            {
                "1" => FunctionComponent.Root,
                "2" => FunctionComponent.Second,
                "3" => FunctionComponent.Third,
                "4" => FunctionComponent.Fourth,
                "5" => FunctionComponent.Fifth,
                "6" => FunctionComponent.Sixth,
                "7" => FunctionComponent.Seventh,
                "9" => FunctionComponent.Ninth,
                _ => null
            };
        }

        public static List<FunctionComponent>? GetManyFunctionComponents(List<string> componentList)
        {
            List<FunctionComponent>? result = [];

            foreach (var component in componentList)
            {
                var toAdd = GetFunctionComponent(component);

                if (toAdd != null)
                    result.Add(toAdd);
            }

            return result.Count != 0 ? result : null;
        }

        // TODO
        public static Alteration GetAlteration()
        {
            throw new NotImplementedException();
        }
    }
}
