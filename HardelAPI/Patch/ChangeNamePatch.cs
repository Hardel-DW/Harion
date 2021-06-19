using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace HardelAPI.Patch {
    internal static class ChangeNamePatch {
        internal static void Initialize() {
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, sceneLoadMode) => {
                if (scene.name == "MMOnline") {
                    AddChangeNameInput();
                    UpdatePosition();
                }
            }));
        }

        internal static void AddChangeNameInput() {
            GameObject InputTemplate = DestroyableSingleton<AccountManager>.Instance.accountTab.editNameScreen.nameText.gameObject;
            GameObject Input = Object.Instantiate(InputTemplate);

            Input.name = "Change Name Input";
            Input.transform.localPosition = new Vector3(0f, 2.5f, 0f);

            TextMeshPro TMP = Input.transform.GetChild(1).GetComponent<TextMeshPro>();
            TMP.transform.localPosition = Vector3.zero;
            TMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
            TMP.fontStyle = FontStyles.Bold;
            TMP.fontSize = 3.5f;

            TextBoxTMP textBox = Input.GetComponent<TextBoxTMP>();
            textBox.SetText(SaveManager.PlayerName);
            textBox.OnChange.AddListener((Action) (() => {
                SaveManager.PlayerName = textBox.text;
            }));
        }

        internal static void UpdatePosition() {
            GameObject.Find("NormalMenu/HostGameButton").transform.localPosition += Vector3.down * 0.5f;
            GameObject.Find("NormalMenu/FindGameButton").transform.localPosition += Vector3.down * 0.5f;
            GameObject.Find("NormalMenu/JoinGameButton").transform.localPosition += Vector3.down * 0.5f;
        }
    }
}
