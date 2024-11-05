using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.New.Utils
{
    public static class Permutations
    {
        public static List<List<T>> CreatePermutations<T>(List<T> set, int n)
        {
            var result = new List<List<T>>();
            CreatePermutationsRecursive(set, new List<T>(), result, n, 0);
            return result;
        }

        private static void CreatePermutationsRecursive<T>(List<T> set, List<T> current, List<List<T>> result, int n, int startIndex)
        {
            if (current.Count == n)
            {
                result.Add(new List<T>(current));
                return;
            }

            for (int i = startIndex; i < set.Count; i++)
            {
                current.Add(set[i]);
                CreatePermutationsRecursive(set, current, result, n, i + 1);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}
