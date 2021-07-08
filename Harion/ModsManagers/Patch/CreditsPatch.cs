using Harion.Utility.Helper;
using Harion.Utility.Utils;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Harion.ModsManagers.Patch {

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class CreditsPatch {

        public static void Prefix() {
            GameObject LocalButton = GameObject.Find("PlayLocalButton");

            GameObject CreditsButton = Object.Instantiate(LocalButton);
            CreditsButton.name = "CreditsButton";
            CreditsButton.transform.localPosition = new Vector3(1.025f, 0f, CreditsButton.transform.localPosition.z);
            Object.Destroy(CreditsButton.GetComponent<ImageTranslator>());
            GameObject TMPText = CreditsButton.FindObject("Text_TMP");
            TMPText.GetComponent<TextMeshPro>().text = "Credits";
            Object.Destroy(TMPText.GetComponent<TextTranslatorTMP>());

            PassiveButton[] Passives = CreditsButton.gameObject.GetComponents<PassiveButton>();
            foreach (PassiveButton passive in Passives)
                Object.Destroy(passive);

            PassiveButton ButtonPassiveLeft = CreditsButton.AddComponent<PassiveButton>();
            ButtonPassiveLeft.OnClick.RemoveAllListeners();
            ButtonPassiveLeft.OnClick.AddListener((UnityAction) OnClick);
            ButtonPassiveLeft.OnMouseOver = new UnityEvent();
            ButtonPassiveLeft.OnMouseOver.AddListener((UnityAction) OnMouseOver);
            ButtonPassiveLeft.OnMouseOut = new UnityEvent();
            ButtonPassiveLeft.OnMouseOut.AddListener((UnityAction) OnMouseOut);

            void OnClick() => PopupMessage.PopupText("Comming Soon", true);
            void OnMouseOver() => CreditsButton.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 1f);
            void OnMouseOut() => CreditsButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1, 1f);
        }
    }
}