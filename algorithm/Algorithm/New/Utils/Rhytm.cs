namespace Algorithm.New.Utils
{
    public static class Rhytm
    {        
        public static List<int> GetRhytmicScheme(int functionsInBar, int metreCount, int metreValue)
        {
            List<int> result = [];

            if (functionsInBar > 8)
                throw new ArgumentException("Maximum 8 functions in bar.");

            if (metreCount == 3 && metreValue == 8 && functionsInBar > 6)
                throw new ArgumentException("Maximum 6 functions in 3/8 bar.");

            var valueToMultipler = metreValue switch
            {
                4 => 1,
                8 => 4,
                _ => throw new ArgumentException("Invalid metre value")
            };

            var baseValue = (metreValue * metreCount) / valueToMultipler;
            result.Add(baseValue);

            for (int index = 0; index < functionsInBar - 1; index++)
            {
                var allSame = SameElementsInList(result);

                if (allSame)
                {
                    var last = result.Last();
                    var lastDivided = DivideNote(last, metreValue);

                    result[^1] = lastDivided.Item1; // [^1] to jak [-1] w pythonie
                    result.Add(lastDivided.Item2);
                }
                else
                {
                    var maxIndex = result.LastIndexOf(result.Max());
                    var max = result[maxIndex];
                    var divided = DivideNote(max, metreValue);

                    result[maxIndex] = divided.Item1;
                    result.Insert(maxIndex + 1, divided.Item2);
                }
            }

            if (result.Count != functionsInBar)
                throw new Exception("Something went wrong...");

            return result;
        }

        private static (int, int) DivideNote(int rhytmicValue, int metreValue)
        {
            if (rhytmicValue == 12 && metreValue == 8)
                return (6, 6);

            return rhytmicValue switch
            {
                16 => (8, 8),
                12 => (8, 4),
                8 => (4, 4),
                6 => (4, 2),
                4 => (2, 2),
                3 => (2, 1),
                2 => (1, 1),
                _ => throw new ArgumentException("Invalid rhytmic value")
            };
        }

        private static bool SameElementsInList(List<int> list)
        {
            if (list.Count == 0)
                return true;

            var first = list[0];

            foreach (var value in list)
            {
                if (first != value)
                    return false;
            }

            return true;
        }
    }
}
