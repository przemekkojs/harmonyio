using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Generators
{
    public static class ProblemGenerator
    {
        public const int MIN_BARS = 2;

        private const int MAX_WEIGHT = 100;
        private const int MINOR_DEPENDANT = -1;

        private const int PRIORITY_HIGHEST = 80;
        private const int PRIORITY_HIGH = 65;
        private const int PRIORITY_STANDARD = 50;
        private const int PRIORITY_LOW = 35;
        private const int PRIORITY_LOWEST = 20;

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        private static readonly Dictionary<Symbol, List<(int, Symbol)>> NextSymbols = new()
        {
            { Symbol.T, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.S ),
                    ( PRIORITY_HIGHEST, Symbol.Sii ),
                    ( PRIORITY_HIGHEST, Symbol.Svi ),
                    ( PRIORITY_STANDARD, Symbol.D ),
                    ( PRIORITY_STANDARD, Symbol.Dvii ),
                    ( PRIORITY_LOW, Symbol.Diii )
                }
            },
            { Symbol.Sii, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.D ),
                    ( PRIORITY_HIGH, Symbol.T ),
                    ( PRIORITY_STANDARD, Symbol.Dvii ),
                    ( PRIORITY_LOW, Symbol.Tvi ),
                    ( PRIORITY_LOWEST, Symbol.Diii ),
                    ( PRIORITY_LOWEST, Symbol.Tiii )
                }
            },
            { Symbol.Tiii, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.D ),
                    ( PRIORITY_HIGHEST, Symbol.Dvii ),
                    ( PRIORITY_HIGH, Symbol.Svi )
                }
            },
            { Symbol.Diii, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.Tvi ),
                    ( PRIORITY_HIGH, Symbol.T ),
                    ( PRIORITY_HIGH, Symbol.D ),
                    ( PRIORITY_STANDARD, Symbol.Dvii )
                }
            },
            { Symbol.S, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.D ),
                    ( PRIORITY_HIGHEST, Symbol.Dvii ),
                    ( PRIORITY_HIGH, Symbol.T ),
                    ( PRIORITY_STANDARD, Symbol.Tvi )
                }
            },
            { Symbol.D, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.T ),
                    ( PRIORITY_HIGH, Symbol.Tvi )
                }
            },
            { Symbol.Tvi, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.Sii ),
                    ( PRIORITY_HIGH, Symbol.S )
                }
            },
            { Symbol.Svi, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.Diii ),
                    ( PRIORITY_HIGH, Symbol.S ),
                    ( PRIORITY_STANDARD, Symbol.T ),
                    ( PRIORITY_STANDARD, Symbol.D )
                }
            },
            { Symbol.Dvii, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.T ),
                    ( PRIORITY_HIGH, Symbol.Tiii ),
                    ( PRIORITY_STANDARD, Symbol.Tvi )
                }
            },
            { Symbol.Svii, new List<(int, Symbol)>()
                {
                    ( PRIORITY_HIGHEST, Symbol.D ),
                    ( PRIORITY_HIGHEST, Symbol.Diii ),
                    ( PRIORITY_HIGH, Symbol.Tiii )
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

        private static readonly Dictionary<Symbol, List<(int, Component)>> ComponentWeights = new()
        {
            {
                Symbol.T,
                [
                    (0, Component.Sixth),
                    (PRIORITY_LOWEST, Component.Seventh),
                    (PRIORITY_LOWEST, Component.Ninth)
                ]
            },
            {
                Symbol.Sii,
                [
                    (0, Component.Sixth),
                    (PRIORITY_STANDARD, Component.Seventh),
                    (PRIORITY_LOW, Component.Ninth)
                ]
            },
            {
                Symbol.Tiii,
                [
                    (0, Component.Sixth),
                    (PRIORITY_STANDARD, Component.Seventh),
                    (PRIORITY_LOW, Component.Ninth)
                ]
            },
            {
                Symbol.Diii,
                [
                    (0, Component.Sixth),
                    (PRIORITY_HIGH, Component.Seventh),
                    (PRIORITY_STANDARD, Component.Ninth)
                ]
            },
            {
                Symbol.S,
                [
                    (0, Component.Sixth),
                    (PRIORITY_LOW, Component.Seventh),
                    (PRIORITY_LOWEST, Component.Ninth)
                ]
            },
            {
                Symbol.D,
                [
                    (PRIORITY_STANDARD, Component.Sixth),
                    (PRIORITY_HIGHEST, Component.Seventh),
                    (PRIORITY_HIGH, Component.Ninth)
                ]
            },
            {
                Symbol.Tvi,
                [
                    (0, Component.Sixth),
                    (PRIORITY_LOW, Component.Seventh),
                    (PRIORITY_LOWEST, Component.Ninth)
                ]
            },
            {
                Symbol.Svi,
                [
                    (0, Component.Sixth),
                    (PRIORITY_STANDARD, Component.Seventh),
                    (PRIORITY_LOW, Component.Ninth)
                ]
            },
            {
                Symbol.Dvii,
                [
                    (0, Component.Sixth),
                    (PRIORITY_STANDARD, Component.Seventh),
                    (PRIORITY_LOW, Component.Ninth)
                ]
            },
            {
                Symbol.Svii,
                [
                    (0, Component.Sixth),
                    (PRIORITY_LOW, Component.Seventh),
                    (PRIORITY_LOWEST, Component.Ninth)
                ]
            }
        };

        public static List<Function> Generate(int bars, int metreValue, int metreCount, int sharpsCount, int flatsCount, int minor)
        {
            var metre = Metre.GetMetre(metreCount, metreValue);
            var tonationList = Tonation.GetTonation(sharpsCount, flatsCount);
            var isMinor = minor != 1;

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
            // Przy okazji check na ujemne wartości
            if (bars < MIN_BARS)
                bars = MIN_BARS;

            List<Function> result = [];
            Function? current = null;

            var maxFunctionsInBar = metre.Count + 1;

            for (int barIndex = 0; barIndex < bars; barIndex++)
            {
                int functionsInBar = barIndex == bars - 1 ?
                    1 : // Ostatni takt powinien zawierać zawsze 1 funkcję
                    _random.Next(1, maxFunctionsInBar);

                for (int functionIndex = 0; functionIndex < functionsInBar; functionIndex++)
                {
                    current = Next(current, metre, tonation, barIndex, functionIndex);
                    result.Add(current);
                }
            }

            var lastFunction = result.Last();
            AddMissing(lastFunction, metre, tonation, result);

            return result;
        }

        private static void AddMissing(Function lastFunction, Metre metre, Tonation tonation, List<Function> result)
        {
            // Logika dodawania dobrego zakończenia zadania
            if (lastFunction.Symbol != Symbol.T)
            {
                var lastBar = lastFunction.Index.Bar;

                // Jeżeli nie ma dominanty, to trzeba z tym coś zrobić
                if (!lastFunction.Symbol.ToString()[0].Equals("D"))
                {
                    var dominant = new Function(
                        new Music.Index()
                        {
                            Bar = lastBar,
                            Position = 0,
                            Duration = 1
                        },
                        Symbol.D,
                        false,
                        tonation
                    );

                    var tonic = new Function(
                        new Music.Index()
                        {
                            Bar = lastBar,
                            Position = 0,
                            Duration = metre.Value
                        },
                        Symbol.T,
                        false,
                        tonation
                    );

                    result.AddRange([dominant, tonic]);
                }
                else // Jeżeli mamy jakąś dominantę, to wszystko git - tylko tonikę dodać
                {
                    var tonic = new Function(
                        new Music.Index()
                        {
                            Bar = lastBar,
                            Position = metre.Value,
                            Duration = metre.Value
                        },
                        Symbol.T,
                        false,
                        tonation
                    );

                    result.Add(tonic);
                }
            }
        }

        // Ta funkcja dodatkowo reaguje, jeżeli powstaje próba 
        // UŻYWAĆ TYLKO W PĘTLI GENERUJĄCEJ KOLEJNE
        private static Function Next(Function? prev, Metre metre, Tonation tonation, int barIndex, int functionIndex)
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
                result = GetBestFittingFunction(prev, metre, tonation, barIndex, functionIndex);

            // TODO: Dodać tworzenie symboli dodanych w razie potrzeby
            // Na potrzeby bazowe można to pominąć - będą generowane zadania funkcyjne na poziomie I semestru
            // 4 klasy SM II stopnia

            return result;
        }

        // Ta funkcja, z wykorzystaniem najlepiej dopasowanego symbolu, dorabia resztę informacji,
        // by kolejna funkcja dobrze działała.
        // NIE UŻYWAĆ SAMEJ
        private static Function GetBestFittingFunction(Function prev, Metre metre, Tonation tonation, int barIndex, int functionIndex)
        {
            var newBar = barIndex;
            var newPosition = functionIndex;
            var newDuration = metre.Value;

            var prevSymbol = prev.Symbol;
            var possibleSymbols = NextSymbols[prevSymbol];
            var bestSymbol = GetBestFittingSymbol(possibleSymbols);

            var minor = MinorWeights[bestSymbol] == MINOR_DEPENDANT ?
                tonation.Mode == Mode.Minor :
                _random.Next() <= MinorWeights[bestSymbol];            

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
        private static Symbol GetBestFittingSymbol(List<(int, Symbol)> possibleSymbols)
        {
            var ordered = possibleSymbols
                .OrderByDescending(x => x.Item1);

            var minWeight = ordered
                .Min(x => x.Item1);

            var weight = _random.Next(
                minValue: minWeight,
                maxValue: MAX_WEIGHT
            );

            var matching = ordered
                .First(x => x.Item1 <= weight);

            var symbol = matching.Item2;

            return symbol;
        }
    }
}
