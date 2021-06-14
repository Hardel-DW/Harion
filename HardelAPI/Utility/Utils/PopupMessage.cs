using HardelAPI.Reactor;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace HardelAPI.Utility.Utils {
    public static class PopupMessage {

        public static GenericPopup PopUp;
        public static GameObject Button;
        public static SpriteRenderer Renderer;

        public static GameObject LinkContainer;
        public static GameObject Follow;
        public static GameObject Deny;
        public static TextMeshPro FollowTMP;
        public static TextMeshPro DenyTMP;

        internal static void Initialize() {
            DiscordManager discord = DestroyableSingleton<DiscordManager>.Instance;
            PopUp = Object.Instantiate<GenericPopup>(discord.discordPopup).DontDestroy();
            PopUp.gameObject.name = "Popup Generic";
            Button = PopUp.gameObject.transform.Find("ExitGame").gameObject;
            Renderer = PopUp.gameObject.transform.Find("Background").gameObject.GetComponent<SpriteRenderer>();
            Renderer.size = new Vector3(7f, 2f);
            PopUp.TextAreaTMP.enableAutoSizing = false;
            PopUp.TextAreaTMP.fontSize = 2;

            LinkContainer = new GameObject { layer = 5, name = "Container Button" };
            LinkContainer.transform.SetParent(PopUp.gameObject.transform);
            LinkContainer.transform.localPosition = Vector3.zero;

            Follow = Object.Instantiate(Button, LinkContainer.transform);
            Follow.transform.localPosition = new Vector3(-1f, -0.340f, -0.5f);
            FollowTMP = Follow.transform.Find("Text_TMP").gameObject.GetComponent<TextMeshPro>();

            Deny = Object.Instantiate(Button, LinkContainer.transform);
            Deny.transform.localPosition = new Vector3(1f, -0.340f, -0.5f);
            DenyTMP = Deny.transform.Find("Text_TMP").gameObject.GetComponent<TextMeshPro>();

            LinkContainer.SetActive(false);
        }

        private static bool ItsInitialize() {
            if (PopUp == null)
                Initialize();

            return PopUp != null;
        }

        private static void Show(string text) {
            if (ItsInitialize()) {
                PopUp.Show(text);

                if (PopUp.TextAreaTMP != null)
                    PopUp.TextAreaTMP.text = text;

                PopUp.gameObject.SetActive(true);
            }
        }

        public static void Close() {
            if (ItsInitialize())
                PopUp.Close();
        }

        public static void PopupText(string text, bool ShowButton) {
            if (ItsInitialize()) {
                LinkContainer.SetActive(false);
                Button.SetActive(ShowButton);
                Show(text);
            }
        }

        public static void PopupLink(string text, string URL) {
            if (ItsInitialize()) {

                Coroutines.Start(UpdateText());
                PassiveButton[] PassivesButtons = Follow.GetComponents<PassiveButton>();
                foreach (PassiveButton Passivesutton in PassivesButtons)
                    Object.Destroy(Passivesutton);

                PassiveButton passive = Follow.AddComponent<PassiveButton>();
                passive.OnClick.RemoveAllListeners();
                passive.OnClick.AddListener((UnityAction) OnClick);
                passive.OnMouseOver = new UnityEvent();
                passive.OnMouseOver.AddListener((UnityAction) OnMouseOver);
                passive.OnMouseOut = new UnityEvent();
                passive.OnMouseOut.AddListener((UnityAction) OnMouseOut);

                LinkContainer.SetActive(true);
                Button.SetActive(false);
                Show(text);
            }

            void OnMouseOver() => Follow.GetComponent<SpriteRenderer>().color = new Color(0.3f, 1f, 0.3f, 1f);
            void OnMouseOut() => Follow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1, 1f);
            void OnClick() => System.Diagnostics.Process.Start(URL);
        }

        private static IEnumerator UpdateText() {
            yield return new WaitForSeconds(0.01f);
            DenyTMP.SetText("Deny");
            FollowTMP.text = "Continue";
            yield return true;
        }
    }
}
