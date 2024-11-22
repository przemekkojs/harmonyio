using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Generators
{
    public static class ProblemGenerator
    {
        private const int MAX_WEIGHT = 100;
        private const int MINOR_DEPENDANT = -1;

        private const int PRIORITY_HIGHEST = 80;
        private const int PRIORITY_HIGH = 65;
        private const int PRIORITY_STANDARD = 50;
        private const int PRIORITY_LOW = 35;
        private const int PRIORITY_LOWEST = 20;

        private static readonly Random _random = new();

        // TODO: Implementacja
        private static readonly Dictionary<Symbol, Dictionary<int, Symbol>> NextSymbols = new()
        {
            { Symbol.T, new Dictionary<int, Symbol>()
                {
                    { PRIORITY_HIGHEST, Symbol.S },
                    { PRIORITY_HIGHEST, Symbol.Sii },
                    { PRIORITY_HIGHEST, Symbol.Svi },
                    { PRIORITY_STANDARD, Symbol.D },
                    { PRIORITY_STANDARD, Symbol.Dvii },
                    { PRIORITY_LOW, Symbol.Diii }
                }
            },
            { Symbol.Sii, new Dictionary<int, Symbol>()
                {                    
                    { PRIORITY_HIGHEST, Symbol.D },                                
                    { PRIORITY_HIGH, Symbol.T },
                    { PRIORITY_STANDARD, Symbol.Dvii },
                    { PRIORITY_LOW, Symbol.Tvi },
                    { PRIORITY_LOWEST, Symbol.Diii },
                    { PRIORITY_LOWEST, Symbol.Tiii }
                }
            },
            { Symbol.Tiii, new Dictionary<int, Symbol>()
                {
                    { PRIORITY_HIGHEST, Symbol.D },
                    { PRIORITY_HIGHEST, Symbol.Dvii },
                    { PRIORITY_HIGH, Symbol.Svi }
                }
            },
            { Symbol.Diii, new Dictionary<int, Symbol>()
                {

                }
            },
            { Symbol.S, new Dictionary<int, Symbol>()
                {
                    { PRIORITY_HIGHEST, Symbol.D },
                    { PRIORITY_HIGHEST, Symbol.Dvii },
                    { PRIORITY_HIGH, Symbol.T },
                    { PRIORITY_STANDARD, Symbol.T }
                }
            },
            { Symbol.D, new Dictionary<int, Symbol>()
                {
                    { PRIORITY_HIGHEST, Symbol.T },
                    { PRIORITY_HIGH, Symbol.Tvi }
                }
            },
            { Symbol.Tvi, new Dictionary<int, Symbol>()
                {

                }
            },
            { Symbol.Svi, new Dictionary<int, Symbol>()
                {

                }
            },
            { Symbol.Dvii, new Dictionary<int, Symbol>()
                {

                }
            },
            { Symbol.Svii, new Dictionary<int, Symbol>()
                {

                }
            }
        };

        private static readonly Dictionary<Symbol, int> MinorWeights = new()
        {
            { Symbol.T, MINOR_DEPENDANT },
            { Symbol.Sii, 90 },
            { Symbol.Tiii, MINOR_DEPENDANT },
            { Symbol.Diii, 75 },
            { Symbol.S, 50 },
            { Symbol.D, 25 },
            { Symbol.Tvi, MINOR_DEPENDANT },
            { Symbol.Svi, 90 },
            { Symbol.Dvii, 50 },
            { Symbol.Svii, 50 },
        };

        public static List<Function> Generate(int bars, int metreValue, int metreCount, int sharpsCount, int flatsCount, int minor)
        {
            var metre = Metre.GetMetre(metreCount, metreValue);
            var tonationList = Tonation.GetTonation(sharpsCount, flatsCount);
            var isMinor = minor == 1;

            var tonation = isMinor ?
                tonationList.Where(x => x.Mode == Mode.Minor).First() :
                tonationList.Where(x => x.Mode == Mode.Major).First();

            return Generate(bars, metre, tonation);
        }

        // W tej funkcji powinna się też znaleźć obsługa wtrąceń, kiedy już dodatkowe funkcjonalności będą
        // implementowane. Na ten moment system generacji jest dość prosty.
        //
        // Używać jej również można, ale dla celów praktycznych, raczej pierwsze przeładowanie będzie preferowane,
        // ponieważ JSONy zawierają proste informacje, a nie obiekty.
        public static List<Function> Generate(int bars, Metre metre, Tonation tonation)
        {
            List<Function> result = [];
            Function? current = null;

            var maxFunctionsInBar = metre.Count;

            for (int barIndex = 0; barIndex < bars; barIndex++)
            {
                var functionsInBar = _random.Next(1, maxFunctionsInBar);

                for (int functionIndex = 0; functionIndex < functionsInBar; functionIndex++)
                {
                    current = Next(current, metre, tonation);
                }
            }

            return result;
        }

        // Ta funkcja dodatkowo reaguje, jeżeli powstaje próba 
        // UŻYWAĆ TYLKO W PĘTLI GENERUJĄCEJ KOLEJNE
        private static Function Next(Function? prev, Metre metre, Tonation tonation)
        {
            Function result;

            if (prev == null)
            {
                result = new Function(
                    index: new Music.Index()
                    {
                        Bar = 0,
                        Position = 0,
                        Duration = metre.Value,
                    },
                    symbol: Symbol.T,
                    minor: tonation.Mode == Mode.Minor,
                    tonation: tonation
                );                
            }
            else
                result = GetBestFittingFunction(prev, metre, tonation);

            // TODO: Dodać tworzenie symboli dodanych w razie potrzeby
            // Na potrzeby bazowe można to pominąć - będą generowane zadania funkcyjne na poziomie I semestru
            // 4 klasy SM II stopnia

            return result;
        }

        // Ta funkcja, z wykorzystaniem najlepiej dopasowanego symbolu, dorabia resztę informacji,
        // by kolejna funkcja dobrze działała.
        // NIE UŻYWAĆ SAMEJ
        private static Function GetBestFittingFunction(Function prev, Metre metre, Tonation tonation)
        {
            var prevIndex = prev.Index;
            var newBar = prevIndex.Bar;
            var newPosition = prevIndex.Position + metre.Value;
            var newDuration = metre.Value;

            if (newPosition >= metre.Value * metre.Count)
            {
                newPosition = 0;
                newBar++;
            }

            var prevSymbol = prev.Symbol;
            var possibleSymbols = NextSymbols[prevSymbol];
            var bestSymbol = GetBestFittingSymbol(possibleSymbols);

            var minor = MinorWeights[bestSymbol] == MINOR_DEPENDANT ?
                tonation.Mode == Mode.Minor :
                _random.Next() >= MinorWeights[bestSymbol];            

            return new Function(
                index: new Music.Index()
                {
                    Bar = newBar,
                    Position = newPosition,
                    Duration = newDuration
                },
                symbol: bestSymbol,
                minor: minor,
                tonation: tonation
            );
        }

        // Ta funkcja zwraca najlepiej pasujący symbol dla kolejnej funkcji.
        // NIE UŻYWAĆ SAMEJ
        private static Symbol GetBestFittingSymbol(Dictionary<int, Symbol> possibleSymbols)
        {
            var keys = possibleSymbols.Keys;
            var minWeight = keys
                .Min(x => x);

            var weight = _random.Next(
                minValue: minWeight,
                maxValue: MAX_WEIGHT
            );

            // Zawsze będzie min. jeden element, bo weight jest losowana od najmniejszej wagi w zbiorze.
            var matchingKey = possibleSymbols.Keys
                .Where(x => weight >= x)
                .OrderBy(x => x)
                .First();

            var symbol = possibleSymbols[matchingKey];
            return symbol;
        }
    }
}
