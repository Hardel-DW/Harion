using HarmonyLib;
using System.Linq;

namespace HardelAPI.Data.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
    class RevivePatch {
        public static void Prefix(PlayerControl __instance) {
            if (DeadPlayer.deadPlayers.Any(p => p.player.PlayerId == __instance.PlayerId)) {
                DeadPlayer deadPlayersToRemove = DeadPlayer.deadPlayers.FirstOrDefault(p => p.player.PlayerId == __instance.PlayerId);
                DeadPlayer.deadPlayers.Remove(deadPlayersToRemove);
            }
        }
    }
}