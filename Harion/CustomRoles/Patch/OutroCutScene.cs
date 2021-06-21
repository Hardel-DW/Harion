using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class OutroCutScene {
        public static void Postfix(EndGameManager __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                if (Role.HasWin) {
                    __instance.BackgroundBar.material.color = Role.Color;

                    TextMeshPro textMeshPro = Object.Instantiate<TextMeshPro>(__instance.WinText);
                    textMeshPro.text = Role.OutroDescription;
                    textMeshPro.color = Role.Color;

                    Vector3 localPosition = __instance.WinText.transform.localPosition;
                    localPosition.y = 1.5f;
                    textMeshPro.transform.position = localPosition;
                    textMeshPro.text = textMeshPro.text;
                    textMeshPro.fontSize = 4;

                    Role.HasWin = false;
                    RoleManager.WinPlayer = new List<PlayerControl>();
                }
            }
        }
    }
}