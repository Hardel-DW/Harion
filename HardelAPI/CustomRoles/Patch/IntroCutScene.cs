using HarmonyLib;
using UnityEngine;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch]
    class IntroCutScenePatch {
        public static void IntroCutSceneRole(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
            RoleManager Role = RoleManager.GetMainRole(PlayerControl.LocalPlayer);
            
            if (Role != null) {
                Role.DefineIntroTeam(ref yourTeam);
                Role.OnIntroCutScene();

                if (Role.ShowIntroCutScene) {
                    __instance.Title.text = Role.Name;
                    __instance.Title.m_fontScale /= 1 + (Mathf.Max(0f, Role.Name.Length - 5) * (1 / 3));
                    __instance.Title.color = Role.Color;
                    __instance.ImpostorText.text = Role.IntroDescription;
                    __instance.BackgroundBar.material.color = Role.Color;
                }
            }
        }


        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        class BeginCrewmatePatch {
            public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                IntroCutSceneRole(__instance, ref yourTeam);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        class BeginImpostorPatch {
            public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam) {
                IntroCutSceneRole(__instance, ref yourTeam);
            }
        }
    }
}
