using HarmonyLib;
using UnityEngine;
using Harion.Utility.Utils;

namespace Harion.CustomKeyBinds.Patch {
    internal static class KeyboardJoystickPatches {

        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static class PatchMainMenuManagerUpdate {

            public static bool Prefix(KeyboardJoystick __instance) {
                if (!PlayerControl.LocalPlayer)
                    return false;

                Vector2 del = Vector2.zero;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyBindPatch.Right.Key))
                    del.x += 1f;
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyBindPatch.Left.Key))
                    del.x -= 1f;
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyBindPatch.Forward.Key))
                    del.y += 1f;
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyBindPatch.Backward.Key))
                    del.y -= 1f;

                del.Normalize();
                __instance.del = del;

                HandleHud();
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    if (Minigame.Instance)
                        Minigame.Instance.Close();
                    else if (DestroyableSingleton<HudManager>.InstanceExists && MapBehaviour.Instance &&
                             MapBehaviour.Instance.IsOpen)
                        MapBehaviour.Instance.Close();
                    else if (CustomPlayerMenu.Instance)
                        CustomPlayerMenu.Instance.Close(true);
                }

                return false;
            }

            private static void HandleHud() {
                if (!DestroyableSingleton<HudManager>.InstanceExists)
                    return;

                if (Input.GetKeyDown(KeyBindPatch.Report.Key))
                    DestroyableSingleton<HudManager>.Instance.ReportButton.DoClick();

                if (Input.GetKeyDown(KeyBindPatch.Use.Key))
                    DestroyableSingleton<HudManager>.Instance.UseButton.DoClick();

                if (Input.GetKeyDown(KeyBindPatch.Map.Key))
                    DestroyableSingleton<HudManager>.Instance.OpenMap();

                if (Input.GetKeyDown(KeyBindPatch.Tasks.Key))
                    HudInterface.ToggleTab();

                if (PlayerControl.LocalPlayer.Data != null && PlayerControl.LocalPlayer.Data.IsImpostor &&
                    Input.GetKeyDown(KeyBindPatch.Kill.Key))
                    DestroyableSingleton<HudManager>.Instance.KillButton.PerformKill();
            }
        }
    }
}
