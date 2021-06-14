using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardelAPI.Patch {
    
    [HarmonyPatch(typeof(Constants), nameof(Constants.ShouldFlipSkeld))]
    class Dleks {
        public static bool Prefix(ref bool __result) {
            if (PlayerControl.GameOptions == null)
                return true;

            __result = PlayerControl.GameOptions.MapId == 3;
            return false;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    class GameSettingMenuPatch {

        public static void Postfix(GameSettingMenu __instance) {
            Transform mapOptions = __instance.AllItems.FirstOrDefault(x => x.gameObject.activeSelf && x.name.Equals("MapName", System.StringComparison.OrdinalIgnoreCase));
            if (mapOptions == null)
                return;

            List<KeyValuePair<string, int>> newMaps = new List<KeyValuePair<string, int>>();
            for (int i = 0; i < GameOptionsData.MapNames.Length; i++) {
                KeyValuePair<string, int> map = new KeyValuePair<string, int>();
                map.key = GameOptionsData.MapNames[i];
                map.value = i;
                newMaps.Add(map);
            }
            mapOptions.GetComponent<KeyValueOption>().Values = newMaps;
        }
    }

}
