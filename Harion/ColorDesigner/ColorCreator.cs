using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.ColorDesigner {
    public class ColorCreator {
        internal static Dictionary<int, string> ColorStrings = new Dictionary<int, string>();
        internal static uint pickableColors = (uint) Palette.ColorNames.Length;
        public static List<int> lighterColors = new List<int>() { 3, 4, 5, 7, 10, 11 };
        public static List<CustomColor> colors = new List<CustomColor>();
        private static int LastId = 50000;

        public static void AddColor(CustomColor color) {
            List<StringNames> longlist = Enumerable.ToList(Palette.ColorNames);
            List<Color32> colorlist = Enumerable.ToList(Palette.PlayerColors);
            List<Color32> shadowlist = Enumerable.ToList(Palette.ShadowColors);

            int id = ++LastId;
            longlist.Add((StringNames) id);
            ColorStrings[id++] = color.name;

            colorlist.Add(color.color);
            shadowlist.Add(color.shadow);

            if (color.isLighterColor)
                lighterColors.Add(colorlist.Count - 1);

            Palette.ColorNames = longlist.ToArray();
            Palette.PlayerColors = colorlist.ToArray();
            Palette.ShadowColors = shadowlist.ToArray();
            pickableColors++;
        }

        public static void Load() {
            List<StringNames> longlist = Enumerable.ToList(Palette.ColorNames);
            List<Color32> colorlist = Enumerable.ToList(Palette.PlayerColors);
            List<Color32> shadowlist = Enumerable.ToList(Palette.ShadowColors);

            int id = 50000;
            foreach (CustomColor customColor in colors) {
                longlist.Add((StringNames) id);
                ColorStrings[id++] = customColor.name;
                colorlist.Add(customColor.color);
                shadowlist.Add(customColor.shadow);
                if (customColor.isLighterColor)
                    lighterColors.Add(colorlist.Count - 1);
            }

            Palette.ColorNames = longlist.ToArray();
            Palette.PlayerColors = colorlist.ToArray();
            Palette.ShadowColors = shadowlist.ToArray();
        }
    }
}
