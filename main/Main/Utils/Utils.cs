using Algorithm.New.Algorithm.Mistake.Solution;
using Main.Models;

namespace Main.Utils
{
    public static class Utils
    {
        public static string MistakesToHTML(ICollection<MistakeResult> mistakes)
        {
            if (mistakes == null)
                return string.Empty;

            if (mistakes.Count == 0)
                return string.Empty;

            if (mistakes.Count == 1)
            {
                var mistake = mistakes.First();
                var codes = mistake.MistakeCodes;

                if (codes.Count == 1)
                {
                    var code = codes[0];

                    switch(code)
                    {
                        case Mistake.NO_SOLUTION_CODE:
                        case Mistake.GRADING_MISTAKE_CODE:
                            return Mistake.MistakeCodeToDescription(code);
                        default:
                            break;
                    };
                }
            }

            var tmp = new Dictionary<(int, (int, int, int)), List<string>>();

            foreach (var item in mistakes)
            {
                var barIndexes = item.Bars;
                var functionIndexes = item.Functions;
                var mistakeCodes = item.MistakeCodes;

                var bar1 = barIndexes.Count > 0 ? barIndexes[0] : -1;
                var bar2 = barIndexes.Count > 1 ? barIndexes[1] : bar1;

                var function1 = functionIndexes.Count > 0 ? functionIndexes[0] : -1;
                var function2 = functionIndexes.Count > 1 ? functionIndexes[1] : function1;

                var key = (bar1, (function1, function2, bar2));

                if (!tmp.ContainsKey(key))
                    tmp[key] = [];

                foreach (var mistakeCode in mistakeCodes)
                {
                    var description = Mistake.MistakeCodeToDescription(mistakeCode);
                    tmp[key].Add(description);
                }
            }

            var sortedKeys = tmp.Keys
                .OrderBy(key => key.Item1)
                    .ThenBy(key => key.Item2.Item1)
                        .ThenBy(key => key.Item2.Item2)
                            .ThenBy(key => key.Item2.Item3)
                .ToList();

            int lastBar = 0;
            var result = string.Empty;

            foreach (var key in sortedKeys)
            {
                var bar = key.Item1 + 1;

                if (bar <= 0)
                    bar = 1;

                var function1 = key.Item2.Item1 + 1;
                var function2 = key.Item2.Item2 + 1;
                var bar2 = key.Item2.Item3 + 1;

                if (bar2 <= 0)
                    bar2 = 1;

                if (bar != lastBar)
                {
                    if (lastBar > 0)
                        result += $"</details>";

                    result += $"<details><summary>Takt <b>{bar}</b></summary>";
                }

                lastBar = bar;

                if (function1 == function2)
                    result += $"<details><summary>Funkcja na miarę <b>{function1}</b></summary>";
                else
                {
                    result += (bar == bar2 ?
                        $"<details><summary>Funkcje na miary <b>{function1}</b>, <b>{function2}</b></summary>" :
                        $"<details><summary>Funkcje na miary <b>{function1}</b>, <b>{function2}</b> w takcie <b>{bar2}</b>)</summary>");
                }

                foreach (var o in tmp[key])
                {
                    result += $"<span>{o}</span><br>";
                }

                result += "</details>";
            }

            return result;
        }
    }
}
