using HarmonyLib;
using UnhollowerBaseLib;

namespace HardelAPI.Utility.Helper {

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    public class StringNameHelper {
        public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name) {
            switch ((int) name) {
                case 123456789:
                    __result = "Example";
                    return false;
            };

            return true;
        }
    }
}
