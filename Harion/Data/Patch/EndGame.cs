using HarmonyLib;

namespace Harion.Data.Patch {

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class EndGame {
        public static bool Prefix(EndGameManager __instance) {
            DeadPlayer.ClearDeadPlayer();

            return true;
        }
    }
}
