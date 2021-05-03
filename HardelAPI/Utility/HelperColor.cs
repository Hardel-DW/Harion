using Assets.CoreScripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardelAPI.Utility {
    public class HelperColor {
        public static List<int> lighterColors = new List<int>() { 3, 4, 5, 7, 10, 11 };

        public static void AddNewColor(StringNames shortColorName, StringNames colorName, Color32 color, Color32 shadow, int isLighterColor) {
            var newShortColorNames = Palette.ShortColorNames.ToList();
            var newPlayerColors = Palette.PlayerColors.ToList();
            var newShadowColors = Palette.ShadowColors.ToList();
            var newColorNames = Palette.ColorNames.ToList();
            var newTelemetryColorNames = Telemetry.ColorNames.ToList();

            newShortColorNames.Add(shortColorName);
            newColorNames.Add(colorName);
            newPlayerColors.Add(color);
            newShadowColors.Add(shadow);
            newTelemetryColorNames.Add(colorName);

            Palette.ShortColorNames = newShortColorNames.ToArray();
            MedScanMinigame.ColorNames = newColorNames.ToArray();
            Palette.PlayerColors = newPlayerColors.ToArray();
            Palette.ShadowColors = newShadowColors.ToArray();
            Telemetry.ColorNames = newTelemetryColorNames.ToArray();

            lighterColors.Add(isLighterColor);
        }

         public static Color32 Shadow(Color color) {
            Color32 color32 = new Color(color.r - 0.3f, color.g - 0.3f, color.b - 0.3f);
            return color32;
        }

        public static bool isLighterColor(int colorId) {
            return lighterColors.Contains(colorId);
        }
    }
}
