using HarmonyLib;

namespace Harion.ColorDesigner.Patch {

    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadPlayerPrefs))]
    internal static class LoadPlayerPrefsPatch {
        private static bool needsPatch = false;

        public static void Prefix([HarmonyArgument(0)] bool overrideLoad) {
            if (!SaveManager.loaded || overrideLoad)
                needsPatch = true;
        }

        public static void Postfix() {
            if (!needsPatch)
                return;

            SaveManager.colorConfig %= ColorCreator.pickableColors;
            needsPatch = false;
        }
    }
}
