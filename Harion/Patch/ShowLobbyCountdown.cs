using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using System.Windows;
using TMPro;
using Harion.Utility.Utils;

namespace Harion.Patch {

    [HarmonyPatch]
    public class ShowLobbyCountdown {
        private static float timer = 600f;
        private static string GameRoomName = "";
        private static string DisplayedName = "";

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public class ShowLobbyCountdownStart {
            public static void Postfix(GameStartManager __instance) {
                GameRoomName = __instance.GameRoomName.text;

                if (!HarionPlugin.StreamerMode.Value)
                    DisplayedName = GameRoomName;
                else
                    DisplayedName = "Code\n******";

                PassiveButton passiveButton;
                if (__instance.GameRoomName.GetComponent<PassiveButton>() != null) {
                    passiveButton = __instance.GameRoomName.gameObject.GetComponent<PassiveButton>();
                }
                else {
                    BoxCollider2D collider = __instance.GameRoomName.gameObject.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(1.25f, 1f);
                    passiveButton = __instance.GameRoomName.gameObject.AddComponent<PassiveButton>();
                }
                
                passiveButton.OnClick.RemoveAllListeners();
                passiveButton.OnClick.AddListener((UnityAction) OnClick);
                passiveButton.OnMouseOver = new UnityEvent();
                passiveButton.OnMouseOver.AddListener((UnityAction) OnMouseOver);
                passiveButton.OnMouseOut = new UnityEvent();
                passiveButton.OnMouseOut.AddListener((UnityAction) OnMouseOut);

                void OnClick() => ClipboardHelper.PutClipboardString(GameRoomName);
                void OnMouseOver() => __instance.GameRoomName.GetComponent<TextMeshPro>().color = new Color(0.3f, 1f, 0.3f, 1f);
                void OnMouseOut() => __instance.GameRoomName.GetComponent<TextMeshPro>().color = new Color(1f, 1f, 1f, 1f);
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

                if (!HarionPlugin.StreamerMode.Value && DisplayedName.Contains("******"))
                    DisplayedName = GameRoomName;

                if (HarionPlugin.StreamerMode.Value && !DisplayedName.Contains("******"))
                    DisplayedName = "Code\n******";

                timer = Mathf.Max(0f, timer -= Time.deltaTime);
                int minutes = (int) timer / 60;
                int seconds = (int) timer % 60;
                string suffix = $"({minutes:00}:{seconds:00})";

                __instance.GameRoomName.text = $"{DisplayedName}\n{suffix}";
            }
        }
    }
}