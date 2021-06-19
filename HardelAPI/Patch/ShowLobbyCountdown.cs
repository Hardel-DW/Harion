using HarmonyLib;
using UnityEngine;

namespace HardelAPI.Patch {

    [HarmonyPatch]
    public class ShowLobbyCountdown {
        private static float timer = 600f;
        private static string GameRoomName = "";

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public class ShowLobbyCountdownStart {
            public static void Postfix(GameStartManager __instance) {
                GameRoomName = __instance.GameRoomName.text;
                timer = 600f;
            }
        }


        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class ShowLobbyCountdownUpdate {
            public static void Postfix(GameStartManager __instance) {
                if (!AmongUsClient.Instance.AmHost || !GameData.Instance)
                    return;

                if (AmongUsClient.Instance.GameMode == GameModes.LocalGame)
                    return;

                timer = Mathf.Max(0f, timer -= Time.deltaTime);
                int minutes = (int) timer / 60;
                int seconds = (int) timer % 60;
                string suffix = $" ({minutes:00}:{seconds:00})";

                __instance.GameRoomName.text = $"{GameRoomName}\n{suffix}";
            }
        }
    }
}