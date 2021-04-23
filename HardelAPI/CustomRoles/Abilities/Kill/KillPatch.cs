using System;
using HarmonyLib;
using UnityEngine;

namespace HardelAPI.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public static class KillPatch {

        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButtonManager __instance) {
            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsImpostor)
                return true;

            foreach (var Role in RoleManager.AllRoles) {
                KillAbility KillAbility = Role.GetAbility<KillAbility>();
                if (KillAbility == null)
                    continue;

                if (KillAbility.WhiteListKill == null || !Role.HasRole(PlayerControl.LocalPlayer) || KillAbility.WhiteListKill == null)
                    continue;

                PlayerControl ClosestPlayer = KillAbility.GetClosestTarget(PlayerControl.LocalPlayer);
                bool CanKill = Vector2.Distance(PlayerControl.LocalPlayer.transform.position, ClosestPlayer.transform.position) < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
                
                if (KillAbility.KillTimer() == 0f && __instance.enabled && CanKill) {
                    Role.OnLocalAttempKill(PlayerControl.LocalPlayer, ClosestPlayer);
                    KillAbility.LastKilled = DateTime.UtcNow;
                }
            }

            return false;
        }
    }
}
