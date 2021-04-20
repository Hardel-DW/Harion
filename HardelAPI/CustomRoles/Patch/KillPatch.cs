using System;
using HarmonyLib;
using HardelAPI.Utility;
using UnityEngine;

namespace HardelAPI.Utility.CustomRoles.Patch {

    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public static class KillPatch {

        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButtonManager __instance) {
            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsImpostor)
                return true;

            foreach (var Role in RoleManager.AllRoles) {
                if (Role.WhiteListKill == null || !Role.HasRole(PlayerControl.LocalPlayer) || Role.WhiteListKill == null)
                    continue;

                PlayerControl ClosestPlayer = Role.GetClosestTarget(PlayerControl.LocalPlayer);
                bool CanKill = Vector2.Distance(PlayerControl.LocalPlayer.transform.position, ClosestPlayer.transform.position) < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
                
                if (Role.KillTimer() == 0f && __instance.enabled && CanKill) {
                    Role.OnLocalAttempKill(PlayerControl.LocalPlayer, ClosestPlayer);
                    Role.LastKilled = DateTime.UtcNow;
                }
            }

            return false;
        }
    }
}
