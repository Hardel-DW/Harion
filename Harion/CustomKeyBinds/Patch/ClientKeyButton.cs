using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Harion.CustomKeyBinds.Patch;

namespace Harion.CustomKeyBinds.Components {

    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    public class ClientKeyButton {
        private static Vector3? origin;
        private static ToggleButtonBehaviour streamerMods;
        private static GameObject customKeyButton;

        private static void UpdateToggle(ToggleButtonBehaviour button, string text, bool on) {
            if (button == null || button.gameObject == null)
                return;

            Color color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
            button.Background.color = color;
            button.Text.text = $"{text}{(on ? "On" : "Off")}";
            if (button.Rollover)
                button.Rollover.ChangeOutColor(color);
        }

        private static ToggleButtonBehaviour CreateCustomToggle(string text, bool on, Vector3 offset, UnityAction onClick, OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                var button = Object.Instantiate(__instance.CensorChatButton, __instance.CensorChatButton.transform.parent);
                button.transform.localPosition = (origin ?? Vector3.zero) + offset;
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(onClick);
                UpdateToggle(button, text, on);

                return button;
            }
            return null;
        }

        private static GameObject CreateMenuButtonBehaviour(string text, Vector3 offset, UnityAction onClick, OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                ToggleButtonBehaviour button = Object.Instantiate(__instance.CensorChatButton, __instance.CensorChatButton.transform.parent);
                button.transform.localPosition = (origin ?? Vector3.zero) + offset;
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(onClick);
                Object.Destroy(button.GetComponent<ToggleButtonBehaviour>());
                button.Text.text = text;
                button.Background.color = new Color(0.5f, 0.5f, 0.5f);
                if (button.Rollover)
                    button.Rollover.ChangeOutColor(new Color(0.5f, 0.5f, 0.5f));

                return button.gameObject;
            }
            return null;
        }

        public static void Postfix(OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                if (origin == null)
                    origin = __instance.CensorChatButton.transform.localPosition + Vector3.up * 0.25f;
                __instance.CensorChatButton.transform.localPosition = origin.Value + Vector3.left * 1.3f;
            }

            if ((streamerMods == null || streamerMods.gameObject == null)) {
                streamerMods = CreateCustomToggle("Streamer Mode: ", HarionPlugin.StreamerMode.Value, Vector3.right * 1.3f, (UnityAction) streamerModeToggle, __instance);

                void streamerModeToggle() {
                    HarionPlugin.StreamerMode.Value = !HarionPlugin.StreamerMode.Value;
                    UpdateToggle(streamerMods, "Streamer Mode: ", HarionPlugin.StreamerMode.Value);
                }
            }

            if ((customKeyButton == null || customKeyButton.gameObject == null)) {
                customKeyButton = CreateMenuButtonBehaviour("Keybind", Vector3.up * -0.5f, (UnityAction) OnClick, __instance);

                void OnClick() => KeyBindPatch.OpenKeyBindMenu();
            }
        }
    }

    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
    public static class HiddenTextPatch {
        private static void Postfix(TextBoxTMP __instance) {
            bool flag = HarionPlugin.StreamerMode.Value && (__instance.name == "GameIdText" || __instance.name == "IpTextBox" || __instance.name == "PortTextBox");
            if (flag)
                __instance.outputText.text = new string('*', __instance.text.Length);
        }
    }
}
