using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI.HatDesigner {
    public class HatCreator {
        internal static bool modded = false;

        internal static List<CustomHat> allHatsData = new List<CustomHat>();
        public static List<uint> TallIds = new List<uint>();
        protected internal static Dictionary<uint, CustomHat> IdToData = new();

        public static void CreateHats(CustomHat hat) => allHatsData.Add(hat);

        public static void CreateMultipleHats(List<CustomHat> hats) => allHatsData.AddRange(hats);

        internal static HatBehaviour CreateHat(CustomHat hat, int id) {
            HardelApiPlugin.Logger.LogInfo($"Creating Hat: {hat.name}");
            Sprite sprite = hat.sprite;
            HatBehaviour newHat = ScriptableObject.CreateInstance<HatBehaviour>();
            newHat.MainImage = sprite;
            newHat.ProductId = hat.name;
            newHat.Order = 99 + id;
            newHat.InFront = true;
            newHat.NoBounce = !hat.bounce;
            newHat.ChipOffset = hat.offset;

            return newHat;
        }

        private static IEnumerable<HatBehaviour> CreateAllHats() {
            for (int i = 0; i < allHatsData.Count; i++) {
                CustomHat hat = allHatsData[i];
                yield return CreateHat(hat, i);
            }
        }
    }
}
