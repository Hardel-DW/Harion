using HarmonyLib;
using UnhollowerBaseLib;

namespace Harion.ColorDesigner.Patch {
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    class TralsationColorNamePatch {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name) {
            if ((int) name >= 50000) {
                string text = ColorCreator.ColorStrings[(int) name];
                if (text != null) {
                    __result = text;
                    return false;
                }
            }

            return true;
        }
    }
}