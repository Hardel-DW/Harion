using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI.CustomRoles.Abilities.UsableVent {

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatch {
        public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse) {
            PlayerControl player = playerInfo.Object;
            List<RoleManager> AllRoles = RoleManager.GetAllRoles(player);
            bool CanVent = false;

            foreach (var Role in AllRoles) {
                VentAbility ventAbility = Role.GetAbility<VentAbility>();
                if (ventAbility == null)
                    continue;

                CanVent = ventAbility.CanVent;
            }

            if (RoleManager.GetAllRoles(player).Count == 0 && player.Data.IsImpostor)
                CanVent = true;

            float maxFloat = float.MaxValue;
            couldUse = (CanVent && !playerInfo.IsDead && (player.CanMove || player.inVent));
            canUse = couldUse;

            if (__instance.name.StartsWith("SealedVent_")) {
                canUse = couldUse = false;
                __result = maxFloat;
                return false;
            }

            if (canUse) {
                maxFloat = Vector2.Distance(player.GetTruePosition(), __instance.transform.position);
                canUse &= maxFloat <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(player.GetTruePosition(), __instance.transform.position, Constants.ShipAndObjectsMask, false);
            }

            __result = maxFloat;
            return false;
        }
    }
}
