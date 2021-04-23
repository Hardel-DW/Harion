using Assets.CoreScripts;
using System.Linq;
using UnityEngine;

namespace HardelAPI.Utility {
    public class HelperColor {

        public static void AddNewColor(StringNames shortColorName, StringNames colorName, Color32 color, Color32 shadow) {
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
        }

         public static Color32 Shadow(Color color) {
            Color32 color32 = new Color(color.r - 0.3f, color.g - 0.3f, color.b - 0.3f);
            return color32;
        }
    }
}
