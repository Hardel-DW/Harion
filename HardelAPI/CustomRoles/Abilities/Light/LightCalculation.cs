using HarmonyLib;
using System.Collections.Generic;

namespace HardelAPI.CustomRoles.Abilities.Light {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public class LightCalculation {
        public static void Postfix(ref float __result, ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo PlayerData) {
            PlayerControl Player = PlayerData.Object;
            List<RoleManager> AllPlayerRoles = RoleManager.GetAllRoles(Player);
            float LightMultiplier = 1f;
            float LightAdditionnal = 0f;
            bool canSeeDuringLight = false;
            bool lightSabotage = false;

            foreach (var Role in AllPlayerRoles) {
                LightAbility ventAbility = Role.GetAbility<LightAbility>();
                if (ventAbility == null)
                    continue;

                LightMultiplier += ventAbility.LightValueMultiplier;
                LightAdditionnal += ventAbility.LightValueAdditionnal;
                canSeeDuringLight = ventAbility.CanSeeDuringLightSabotage;
            }

            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                if (task.TaskType == TaskTypes.FixLights)
                    lightSabotage = true;

            if ((lightSabotage && canSeeDuringLight) || !lightSabotage)
                __result = (__instance.MaxLightRadius * (Player.Data.IsImpostor ? PlayerControl.GameOptions.ImpostorLightMod : PlayerControl.GameOptions.CrewLightMod * LightMultiplier)) + LightAdditionnal;
        }
    }
}
