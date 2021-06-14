using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HardelAPI.Reactor.Patches {
    internal static class FreeNamePatch {
        [HarmonyPatch(typeof(SetNameText), nameof(SetNameText.Start))]
        public static class NameInputPatch {
            public static void Postfix(SetNameText __instance) {
                if (!__instance)
                    return;

                GameObject nameText = __instance.gameObject;

                TextBoxTMP textBox = nameText.AddComponent<TextBoxTMP>();
                textBox.Background = nameText.GetComponentInChildren<SpriteRenderer>();
                textBox.OnChange = textBox.OnEnter = textBox.OnFocusLost = new Button.ButtonClickedEvent();
                textBox.characterLimit = 10;

                TextMeshPro textMeshPro = nameText.GetComponentInChildren<TextMeshPro>();
                textBox.outputText = textMeshPro;
                textBox.SetText(SaveManager.PlayerName);

                textBox.OnChange.AddListener((Action) (() => {
                    SaveManager.PlayerName = textBox.text;
                }));

                GameObject pipeGameObject = GameObject.Find("Pipe");
                if (pipeGameObject) {
                    GameObject pipe = UnityEngine.Object.Instantiate(pipeGameObject, textMeshPro.transform);
                    pipe.GetComponent<TextMeshPro>().fontSize = 4f;
                    textBox.Pipe = pipe.GetComponent<MeshRenderer>();
                }
            }
        }
    }
}