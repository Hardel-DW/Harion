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

        public static T PickRandom<T>(this IEnumerable<T> source) {
            if (source == null || source.Count() == 0)
                return default;

            IEnumerable<T> RandomElements = source.PickRandom(1);
            if (RandomElements == null)
                return default;
            
            return RandomElements.Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count) {
            if (source.Count() == 0)
                return null;

            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) => source.OrderBy(x => Guid.NewGuid());

        public static Il2CppSystem.Collections.Generic.List<T> CastToIl2Cpp<T>(this List<T> Elements) {
            Il2CppSystem.Collections.Generic.List<T> NewList = new();
            Elements.ForEach(element => NewList.Add(element));
            return NewList;
        }

        public static List<T> CastToList<T>(this Il2CppSystem.Collections.Generic.List<T> Elements) => Elements.ToArray().ToList();
    }
}