using HardelAPI.Utility.Helper;
using HardelAPI.Utility.Utils;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

namespace HardelAPI.ModsManagers.Patch {

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class CreditsPatch {

        public static void Prefix() {
            GameObject LocalButton = GameObject.Find("PlayLocalButton");

            GameObject CreditsButton = Object.Instantiate(LocalButton);
            CreditsButton.name = "CreditsButton";
            CreditsButton.transform.localPosition = new Vector3(1.025f, 0f, CreditsButton.transform.localPosition.z);
            Object.Destroy(CreditsButton.GetComponent<ImageTranslator>());

            SpriteRenderer RendererCredits = CreditsButton.GetComponent<SpriteRenderer>();
            if (RendererCredits != null)
                RendererCredits.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.CreditsButton.png", 100f);

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
            void OnMouseOver() => CreditsButton.GetComponent<SpriteRenderer>().color = new Color(0.3f, 1f, 0.3f, 1f);
            void OnMouseOut() => CreditsButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1, 1f);
        }
    }
}