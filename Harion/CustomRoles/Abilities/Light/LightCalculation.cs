using HarmonyLib;
using System.Collections.Generic;

namespace Harion.CustomRoles.Abilities.Light {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public class LightCalculation {
        public static bool Prefix(ref float __result, ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo PlayerData) {
            if (PlayerData == null || PlayerData.IsDead) {
                __result = __instance.MaxLightRadius;
                return false;
            }

            PlayerControl Player = PlayerData.Object;
            List<RoleManager> AllPlayerRoles = RoleManager.GetAllRoles(Player);
            float LightMultiplier = 1;
            float LightAdditionnal = 0;
            bool canSeeDuringLight = false;
            bool lightSabotage = false;
            bool hasAbility = false;

            foreach (var Role in AllPlayerRoles) {
                LightAbility ventAbility = Role.GetAbility<LightAbility>();
                if (ventAbility == null)
                    continue;

                LightMultiplier += ventAbility.LightValueMultiplier - 1;
                LightAdditionnal += ventAbility.LightValueAdditionnal;
                canSeeDuringLight = ventAbility.CanSeeDuringLightSabotage;
                hasAbility = true;
            }

            if (hasAbility) {
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                    if (task.TaskType == TaskTypes.FixLights)
                        lightSabotage = true;

                if ((lightSabotage && canSeeDuringLight) || !lightSabotage) {
                    float result = (__instance.MaxLightRadius * ((Player.Data.IsImpostor ? PlayerControl.GameOptions.ImpostorLightMod : PlayerControl.GameOptions.CrewLightMod) * LightMultiplier)) + LightAdditionnal;
                    __result = result;
                    return false;
                } else
                    return true;
            }

            return true;
        }
    }
}
