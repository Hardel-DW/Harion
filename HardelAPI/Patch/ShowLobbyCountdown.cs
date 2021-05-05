using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Hazel;
using System;
using UnhollowerBaseLib;

namespace HardelAPI.Patch {

    [HarmonyPatch]
    public class ShowLobbyCountdown {
        private static float timer = 600f;

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public static void Prefix(GameStartManager __instance) {
            timer = 600f;
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public static void Postfix(GameStartManager __instance) {
            if (!AmongUsClient.Instance.AmHost || !GameData.Instance)
                return;

            timer = Mathf.Max(0f, timer -= Time.deltaTime);
            int minutes = (int) timer / 60;
            int seconds = (int) timer % 60;
            string suffix = $" ({minutes:00}:{seconds:00})";

            __instance.PlayerCounter.text = __instance.PlayerCounter.text + suffix;
            __instance.PlayerCounter.autoSizeTextContainer = true;
        }
    }
}