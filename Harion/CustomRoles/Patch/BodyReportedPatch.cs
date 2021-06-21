using Harion.Utility.Utils;
using HarmonyLib;
using UnityEngine;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
    public static class BodyReportedPatch {
        public static void Prefix(PlayerControl __instance) {
            PlayerControl PlayerReported = ClosestReport(__instance);

            foreach (var Role in RoleManager.AllRoles)
                Role.OnBodyReport(__instance, PlayerReported);
        }

        public static PlayerControl ClosestReport(PlayerControl __instance) {
            PlayerControl Player = null;

            if (AmongUsClient.Instance.IsGameOver || __instance.Data.IsDead) {
                return null;
            }

            foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(), __instance.MaxReportDistance, Constants.PlayersOnlyMask)) {
                if (!(collider2D.tag != "DeadBody")) {
                    DeadBody component = collider2D.GetComponent<DeadBody>();
                    Player = PlayerControlUtils.FromPlayerId(component.ParentId);
                }
            }

            return Player;
        }
    }
}
