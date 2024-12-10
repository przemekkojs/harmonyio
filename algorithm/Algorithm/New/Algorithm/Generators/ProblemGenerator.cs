using Algorithm.New.Algorithm.Checkers;
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

        private static readonly Random _random = new(DateTime.Now.Millisecond);

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

        private static readonly List<(Component, int)> RootWeights = new()
        {
            ( Component.Root, PRIORITY_STANDARD ),
            ( Component.Third, PRIORITY_LOW ),
            ( Component.Fifth, PRIORITY_LOW )
        };

        private static readonly List<(Component, int)> PositionWeights = new()
        {
            ( Component.Root, PRIORITY_LOW ),
            ( Component.Third, PRIORITY_LOW ),
            ( Component.Fifth, PRIORITY_LOW )
        };

        /// <summary>
        /// Z tej funkcji generującej najpewniej się będzie korzystać
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="metreValue"></param>
        /// <param name="metreCount"></param>
        /// <param name="sharpsCount"></param>
        /// <param name="flatsCount"></param>
        /// <param name="minor"></param>
        /// <returns></returns>
        public static List<Function> Generate(int bars, int metreValue, int metreCount, int sharpsCount, int flatsCount, int minor)
        {
            var metre = Metre.GetMetre(metreCount, metreValue);
            var tonationList = Tonation.GetTonation(sharpsCount, flatsCount);
            var isMinor = minor != 1;

            var tonation = isMinor ?
                tonationList.Where(x => x.Mode == Mode.Minor).First() :
                tonationList.Where(x => x.Mode == Mode.Major).First();

            var generated = Generate(bars, metre, tonation);

            return generated;
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
            var maxIterationsCount = bars * 4 * 48;
            var currentIteration = 0;

            for (int barIndex = 0; barIndex < bars; barIndex++)
            {
                int functionsInBar = barIndex == bars - 1 ?
                    1 : // Ostatni takt powinien zawierać zawsze 1 funkcję
                    _random.Next(1, maxFunctionsInBar);

                for (int functionIndex = 0; functionIndex < functionsInBar; functionIndex++)
                {
                    var next = Next(current, metre, tonation, barIndex, functionIndex);

                    if (current == null)
                    {
                        current = next;
                        result.Add(current);
                        continue;
                    }

                    // Dopóki są błędy w takim czymś, to nie można raczej tak zrobić
                    int mistakesCount;

                    do
                    {
                        currentIteration++;
                        next = Next(current, metre, tonation, barIndex, functionIndex);
                        var tmpProblem = new Problem([current, next], metre, tonation);
                        var mistakes = ProblemChecker.CheckProblem(tmpProblem);
                        mistakesCount = mistakes.Count;

                        if (currentIteration >= maxIterationsCount)
                            return [];

                    } while (mistakesCount != 0);

                    current = next;
                    result.Add(current);
                }
            }

            var lastFunction = result.Last();
            AddMissing(lastFunction, metre, tonation, result);

            return result;
        }

        /// <summary>
        /// Dodaje brakujące funkcje do zadania, w razie potrzeby
        /// </summary>
        /// <param name="lastFunction"></param>
        /// <param name="metre"></param>
        /// <param name="tonation"></param>
        /// <param name="result"></param>
        private static void AddMissing(Function lastFunction, Metre metre, Tonation tonation, List<Function> result)
        {
            // Logika dodawania dobrego zakończenia zadania
            if (lastFunction.Symbol != Symbol.T)
            {
                // Dla pewności, że nic się nie popsuje
                if (lastFunction.Added.Count > 0)
                {
                    lastFunction.Added = [];
                    lastFunction.Position = null;
                    lastFunction.Root = null;
                    lastFunction.Removed = null;

                    lastFunction.DeductPossibleComponents();
                }                    

                var lastBar = lastFunction.Index.Bar;
                var lastSymbol = lastFunction.Symbol;

                // Jeżeli nie ma dominanty, to trzeba z tym coś zrobić
                if (lastSymbol != Symbol.D)
                {
                    var dominant = new Function(
                        new Music.Index()
                        {
                            Bar = lastBar,
                            Position = lastFunction.Index.Position + 1,
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
                            Position = lastFunction.Index.Position + 2,
                            Duration = 1
                        },
                        Symbol.T,
                        tonation.Mode == Mode.Minor,
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
                            Position = lastFunction.Index.Position + 1,
                            Duration = 1
                        },
                        Symbol.T,
                        tonation.Mode == Mode.Minor,
                        tonation
                    );

                    result.Add(tonic);
                }
            }
            else
            {
                lastFunction.Minor = tonation.Mode == Mode.Minor;
                lastFunction.Added = [];
            }
        }

        /// <summary>
        /// UŻYWAĆ TYLKO W PĘTLI GENERUJĄCEJ KOLEJNE
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="metre"></param>
        /// <param name="tonation"></param>
        /// <param name="barIndex"></param>
        /// <param name="functionIndex"></param>
        /// <returns></returns>
        private static Function Next(Function? prev, Metre metre, Tonation tonation, int barIndex, int functionIndex)
        {
            var result = GetBestFittingFunction(prev, metre, tonation, barIndex, functionIndex);

            AddAddedComponents(result);
            AddRootAndPosition(result);

            try
            {
                result.DeductPossibleComponents();
            }
            catch (Exception) // Trochę na chama, ale działa XD
            {
                result.Added = [];
                result.Position = null;
                result.Root = null;
                result.Removed = null;

                result.DeductPossibleComponents();
            }

            return result;
        }

        /// <summary>
        /// Dodaje składniki dodane do funkcji
        /// </summary>
        /// <param name="function">Funkcja, do której składniki mają zostać dodane</param>
        private static void AddAddedComponents(Function function)
        {
            var symbol = function.Symbol;
            var randomValue = _random.Next(MAX_WEIGHT);

            var toAddWeights = ComponentWeights[symbol];

            var possibleToAdd = toAddWeights
                .Where(x => x.Item1 > randomValue)
                .Select(x => x.Item2)
                .Distinct()
                .ToList();

            foreach (var possible in possibleToAdd)
            {
                if (possible.Equals(Component.Sixth))
                {
                    function.Added.Add(possible);
                    break;
                }

                function.Added.Add(possible);
            }
        }

        /// <summary>
        /// Dodaje oparcie i pozycję do funkcji
        /// </summary>
        /// <param name="function">Funkcja, do której mają być dodane oparcie i pozycja</param>
        private static void AddRootAndPosition(Function function)
        {
            var canAddRoot = function.Added.Count < 2;
            var canAddPosition = function.Added.Count < 1;

            if (canAddRoot)
            {
                var rootRandomValue = _random.Next(MAX_WEIGHT);

                var possibleRoots = RootWeights
                    .Where(x => x.Item2 <= rootRandomValue)
                    .Select(x => x.Item1)
                    .ToList();

                if (possibleRoots.Count != 0)
                {
                    var rootIndex = _random.Next(possibleRoots.Count);
                    var root = possibleRoots[rootIndex];
                    function.Root = root;
                }
            }

            if (canAddPosition)
            {
                var positionRandomValue = _random.Next(MAX_WEIGHT);

                var possiblePositions = PositionWeights
                    .Where(x => x.Item2 <= positionRandomValue)
                    .Select(x => x.Item1)
                    .ToList();

                if (possiblePositions.Count != 0)
                {
                    var positionIndex = _random.Next(possiblePositions.Count);
                    var position = possiblePositions[positionIndex];
                    function.Position = position;
                }
            }
        }

        private static Function GetBestFittingAfterSeventh(Function? prev, Metre metre, Tonation tonation, int barIndex, int functionIndex)
        {
            var newBar = barIndex;
            var newPosition = functionIndex;
            var newDuration = metre.Value;
            var prevSymbol = prev!.Symbol;
            var symbolIndex = Function.SymbolIndexes[prevSymbol];

            List<(int, int)> newIndexes =
            [
                (PRIORITY_HIGHEST, symbolIndex - 4),
                (PRIORITY_LOW, symbolIndex - 2),
                (PRIORITY_LOWEST, symbolIndex + 1)
            ];

            var randomValue = _random.Next(100);
            int matchingCount = 0;
            List<(int, int)> matching;

            do
            {
                matching = newIndexes
                    .Where(x => x.Item1 <= randomValue)
                    .ToList();

                randomValue = _random.Next(100);
                matchingCount = matching.Count;
            } while (matchingCount < 1);


            var randomIndex = _random.Next(matchingCount);
            var selected = matching[randomIndex].Item2;

            var newSymbolIndex = Function.SymbolIndexes
                .Where(x => x.Value == selected)
                .FirstOrDefault()
                .Key;

            return new Function(
                index: new Music.Index()
                {
                    Bar = newBar,
                    Position = newPosition,
                    Duration = newDuration
                },
                symbol: newSymbolIndex,
                minor: tonation.Mode == Mode.Minor,
                tonation: tonation
            );
        }

        private static Function GetBestFittingAfterNinth(Function? prev, Metre metre, Tonation tonation, int barIndex, int functionIndex)
        {
            var newBar = barIndex;
            var newPosition = functionIndex;
            var newDuration = metre.Value;
            var prevSymbol = prev!.Symbol;

            var symbolIndex = Function.SymbolIndexes[prevSymbol];
            var newIndex = symbolIndex - 4;

            if (newIndex < 0)
                newIndex += 7;

            var newSymbolIndex = Function.SymbolIndexes
                .Where(x => x.Value == newIndex)
                .FirstOrDefault()
                .Key;

            return new Function(
                index: new Music.Index()
                {
                    Bar = newBar,
                    Position = newPosition,
                    Duration = newDuration
                },
                symbol: newSymbolIndex,
                minor: tonation.Mode == Mode.Minor,
                tonation: tonation
            );
        }

        private static Function GetFittingAfterSixth(Function? prev, Metre metre, Tonation tonation, int barIndex, int functionIndex)
        {
            var newBar = barIndex;
            var newPosition = functionIndex;
            var newDuration = metre.Value;
            var prevSymbol = prev!.Symbol;

            var symbolIndex = Function.SymbolIndexes[prevSymbol];
            var newIndex = symbolIndex - 3;

            if (newIndex < 0)
                newIndex += 7;

            var newSymbolIndex = Function.SymbolIndexes
                .Where(x => x.Value == newIndex)
                .FirstOrDefault()
                .Key;

            return new Function(
                index: new Music.Index()
                {
                    Bar = newBar,
                    Position = newPosition,
                    Duration = newDuration
                },
                symbol: newSymbolIndex,
                minor: tonation.Mode == Mode.Minor,
                tonation: tonation
            );
        }

        private static Function FunctionAfterNull(Metre metre, Tonation tonation)
        {
            return new Function(
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

        // Ta funkcja, z wykorzystaniem najlepiej dopasowanego symbolu, dorabia resztę informacji,
        // by kolejna funkcja dobrze działała.
        // NIE UŻYWAĆ SAMEJ
        private static Function GetBestFittingFunction(Function? prev, Metre metre, Tonation tonation, int barIndex, int functionIndex)
        {
            if (prev == null)
                return FunctionAfterNull(metre, tonation);

            var newBar = barIndex;
            var newPosition = functionIndex;
            var newDuration = metre.Value;
            var prevSymbol = prev.Symbol;

            var hasSixth = prev.Added
                .Contains(Component.Sixth);

            var hasSeventh = prev.Added
                .Contains(Component.Seventh);

            var hasNinth = prev.Added
                .Contains(Component.Seventh);

            // To są szczególne przypadki, niestety
            if (hasNinth)
                return GetBestFittingAfterNinth(prev, metre, tonation, barIndex, functionIndex);
            else if (hasSeventh)
                return GetBestFittingAfterSeventh(prev, metre, tonation, barIndex, functionIndex);
            else if (hasSixth)
                return GetFittingAfterSixth(prev, metre, tonation, barIndex, functionIndex);

            var possibleSymbols = NextSymbols[prevSymbol];
            var bestSymbol = GetBestFittingSymbol(possibleSymbols);

            var minor = MinorWeights[bestSymbol] == MINOR_DEPENDANT ?
                tonation.Mode == Mode.Minor :
                _random.Next(MAX_WEIGHT) <= MinorWeights[bestSymbol];

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
