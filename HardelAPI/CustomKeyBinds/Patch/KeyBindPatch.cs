using HardelAPI.CustomKeyBinds.Components;
using HarmonyLib;
using UnityEngine;

namespace HardelAPI.CustomKeyBinds.Patch {
    internal static class KeyBindPatch {
        public static CustomKeyBind Forward = CustomKeyBind.AddCustomKeyBind(KeyCode.W, "Forward", "Keybinds");
        public static CustomKeyBind Left = CustomKeyBind.AddCustomKeyBind(KeyCode.A, "Left", "Keybinds");
        public static CustomKeyBind Backward = CustomKeyBind.AddCustomKeyBind(KeyCode.S, "Backward", "Keybinds");
        public static CustomKeyBind Right = CustomKeyBind.AddCustomKeyBind(KeyCode.D, "Right", "Keybinds");
        public static CustomKeyBind Use = CustomKeyBind.AddCustomKeyBind(KeyCode.Space, "Use", "Keybinds");
        public static CustomKeyBind Report = CustomKeyBind.AddCustomKeyBind(KeyCode.R, "Report", "Keybinds");
        public static CustomKeyBind Kill = CustomKeyBind.AddCustomKeyBind(KeyCode.Q, "Kill", "Keybinds");
        public static CustomKeyBind Map = CustomKeyBind.AddCustomKeyBind(KeyCode.Tab, "Map", "Keybinds");
        public static CustomKeyBind Tasks = CustomKeyBind.AddCustomKeyBind(KeyCode.T, "Tasks", "Keybinds");
        public static SettingsWindow keyBindsPopUp;
        private static float DeltaScroll = 0f;
        private static float DeltaScrollTolerance = 3f;
        private static float NextActionTime = 0f;
        private static float Interval = 0.2f;

        public static void OpenKeyBindMenu() {
            keyBindsPopUp.ShowKeySelector();
            keyBindsPopUp.Show();
        }

        [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
        public static class PatchOptionsMenuStart {
            public static void Postfix(OptionsMenuBehaviour __instance) {
                keyBindsPopUp = new SettingsWindow(__instance);
            }
        }

        [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Close))]
        public static class PatchOptionsMenuClose {
            public static void Postfix() {
                keyBindsPopUp?.OnClose();
            }
        }

        [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Update))]
        public static class PatchOptionsMenuUpdate {
            public static bool Prefix(OptionsMenuBehaviour __instance) {
                if (keyBindsPopUp == null)
                    return true;

                if (keyBindsPopUp.IsActive) {
                    DeltaScroll += Input.mouseScrollDelta.y;

                    if (Time.time > NextActionTime) {
                        NextActionTime = Time.time + Interval;
                        DeltaScroll = 0f;
                    }

                    if (DeltaScroll > DeltaScrollTolerance) {
                        DeltaScroll = 0f;
                        keyBindsPopUp.ShowKeySelector(KeyPage.Nothing, MovePage.MoveUp);
                    }

                    if (DeltaScroll < DeltaScrollTolerance * -1) {
                        DeltaScroll = 0f;
                        keyBindsPopUp.ShowKeySelector(KeyPage.Nothing, MovePage.MoveDown);
                    }
                }

                if (KeySelector.CanEscape)
                return true;

                if (Input.GetKeyUp(KeyCode.Escape))
                    KeySelector.CanEscape = true;

                return false;
            }

            public static void Postfix() {
                KeySelector.HudUpdate();
                PopUpWindow.HudUpdate();
            }
        }
    }
}
