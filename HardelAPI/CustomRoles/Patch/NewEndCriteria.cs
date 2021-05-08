using HarmonyLib;
using System.Collections.Generic;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class NewEndCriteria {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason) {
            if (reason == GameOverReason.ImpostorByKill || reason == GameOverReason.HumansByVote || reason == GameOverReason.ImpostorByVote)
                foreach (var role in RoleManager.AllRoles)
                    if (role.AddEndCriteria() && role.AllPlayers?.Count > 0)
                        return false;

            bool CustomEnd = false;
            List<PlayerControl> LoosePlayers = new List<PlayerControl>();
            foreach (var role in RoleManager.AllRoles) {
                if (role.LooseRole) {
                    CustomEnd = true;
                    foreach (var player in role.AllPlayers) {
                        LoosePlayers.Add(player);
                    }
                }
            }

            if (CustomEnd) {
                if (reason == GameOverReason.HumansByVote || reason == GameOverReason.HumansByTask || reason == GameOverReason.HumansDisconnect)
                    LoosePlayers.ForEach(p => p.Data.IsImpostor = true);
                else {
                    foreach (var player in LoosePlayers) {
                        player.RemoveInfected();
                        player.Data.IsImpostor = false;
                    }
                }
            }

            return true;
        }
    }
}
