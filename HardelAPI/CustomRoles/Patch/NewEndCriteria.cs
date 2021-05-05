using HarmonyLib;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class NewEndCriteria {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason) {
            if (reason == GameOverReason.ImpostorByKill || reason == GameOverReason.HumansByVote || reason == GameOverReason.ImpostorByVote) {
                foreach (var role in RoleManager.AllRoles)
                    if (role.AddEndCriteria() && role.AllPlayers?.Count > 0)
                        return false;

                return true;
            }

            return true;
        }
    }
}
