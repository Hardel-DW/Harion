using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace Harion.Patch {
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    public static class MapChoosable {
        static void Prefix(ref GameSettingMenu __instance) {
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }
}
