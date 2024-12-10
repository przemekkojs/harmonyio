namespace Algorithm.New.Utils
{
    public static class Combinations
    {
        public static List<List<T>> Generate<T>(List<T> list)
        {
            var result = new List<List<T>>();
            GenerateRecursive(list, 0, result);
            return result;
        }

        private static void GenerateRecursive<T>(List<T> list, int start, List<List<T>> result)
        {
            if (start == list.Count - 1)
            {
                result.Add(new List<T>(list));
                return;
            }

            for (int i = start; i < list.Count; i++)
            {
                Swap(list, start, i);
                GenerateRecursive(list, start + 1, result);
                Swap(list, start, i);
            }
        }

        private static void Swap<T>(List<T> list, int i, int j) =>
            (list[j], list[i]) = (list[i], list[j]);
    }
}
