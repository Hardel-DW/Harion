using System;
using System.Collections.Generic;
using System.Linq;

namespace Harion.Utility.Utils {
    public static class ListUtils {
        private static Random random = new Random();

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;

            while (n > 1) {
                n--;

                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ShuffleByOrder<T>(this IList<T> list) => list.OrderBy(e => random.Next());
    }
}