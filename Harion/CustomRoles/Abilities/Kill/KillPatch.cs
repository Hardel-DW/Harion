using System;
using HarmonyLib;
using UnityEngine;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public static class KillPatch {

        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButtonManager __instance) {
            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            RoleManager Role = RoleManager.GetMainRole(PlayerControl.LocalPlayer);
            bool HasRole = Role != null;

            if (HasRole) {
                if (PlayerControl.LocalPlayer.Data.IsImpostor) {
                    if (Role.GetType().GetMethod("OnLocalAttempKill").DeclaringType == Role.GetType()) {
                        Role.OnLocalAttempKill(PlayerControl.LocalPlayer, __instance.CurrentTarget);
                        return false;
                    }

                    return true;
                }

                KillAbility KillAbility = Role?.GetAbility<KillAbility>();
                if (KillAbility == null)
                    return false;

                if (KillAbility.WhiteListKill == null)
                    return false;
                
                PlayerControl ClosestPlayer = KillAbility.GetClosestTarget(PlayerControl.LocalPlayer);
                bool CanKill = Vector2.Distance(PlayerControl.LocalPlayer.transform.position, ClosestPlayer.transform.position) < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
                if (KillAbility.KillTimer() == 0f && __instance.enabled && CanKill) {
                    Role.OnLocalAttempKill(PlayerControl.LocalPlayer, ClosestPlayer);
                    KillAbility.LastKilled = DateTime.UtcNow;
                }
            }

            if (PlayerControl.LocalPlayer.Data.IsImpostor)
                return true;

            return false;
        }
    }
}
