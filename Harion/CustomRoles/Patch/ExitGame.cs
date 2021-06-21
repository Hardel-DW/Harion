using Harion.Utility.Utils;
using HarmonyLib;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class EndGamePatch {
        public static void Prefix(AmongUsClient __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.AllPlayers.ClearPlayerList();
                Role.OnGameEnded();
            }
        }
    }
}
