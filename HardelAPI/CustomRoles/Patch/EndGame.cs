using HarmonyLib;
using HardelAPI.Utility;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class EndGameManagerPatch {
        public static bool Prefix(EndGameManager __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.AllPlayers.ClearPlayerList();
                Role.OnGameEnded();
            }

            return true;
        }
    }
}
