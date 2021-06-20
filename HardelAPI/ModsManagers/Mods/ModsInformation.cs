using BepInEx;
using HardelAPI.Utility.Helper;
using HardelAPI.Utility.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace HardelAPI.ModsManagers.Mods {

    internal partial class ModsInformation {

        public static ModsInformation Instance;
        internal List<GameObject> SocialObject = new List<GameObject>();
        internal GameObject Entry;
        internal GameObject CloseButton;
        internal GameObject Background;
        internal GameObject EnableButton;
        internal GameObject Title;
        internal GameObject SocialBackground;
        internal GameObject SocialContainer;
        internal GameObject UpdateText;
        internal GameObject UpdateButtonGO;
        internal TextMeshPro TMP;

        private static bool UpdatedPosition = false;
        internal ModManagerData ModData;
        private MainMenuManager InstanceManager;
        private ButtonRolloverHandler UpdateRollover;
        private Color LerpedBreath = Color.white;

        internal ModsInformation(MainMenuManager instance, GameObject Parent) {
            Entry = Object.Instantiate(instance.Announcement.gameObject, Parent.transform);

            Object.Destroy(Entry.GetComponent<AnnouncementPopUp>());
            Object.Destroy(Entry.transform.Find("announcementsBanner").gameObject);
            InstanceManager = instance;

            Entry.name = $"Mods Information";
            Entry.transform.localPosition = new Vector3(0f, 0f, -30f);
            TMP = Entry.transform.Find("Text_TMP").gameObject.GetComponent<TextMeshPro>();
            Background = Entry.transform.Find("Background").gameObject;
            CloseButton = Entry.transform.Find("CloseButton").gameObject;
            CloseButton.transform.localPosition = new Vector3(-2.750f, 1.9f, 0f);
            SpriteRenderer closeRenderer = CloseButton.GetComponent<SpriteRenderer>();
            if (closeRenderer != null)
                closeRenderer.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Back.png", 100f);

            PassiveButton[] buttonLeft = CloseButton.gameObject.GetComponents<PassiveButton>();
            foreach (PassiveButton passive in buttonLeft)
                Object.Destroy(passive);

            PassiveButton ButtonPassiveLeft = CloseButton.AddComponent<PassiveButton>();
            ButtonPassiveLeft.OnClick.RemoveAllListeners();
            ButtonPassiveLeft.OnClick.AddListener((UnityAction) CloseModsMenu);
            ButtonPassiveLeft.OnMouseOver = new UnityEvent();
            ButtonPassiveLeft.OnMouseOut = new UnityEvent();
            Entry.GetComponent<TransitionOpen>().OnClose.AddListener((UnityAction) CloseModsMenuActive);

            SpriteRenderer renderer = Background.GetComponent<SpriteRenderer>();
            renderer.size = new Vector2(5f, 5.5f);
            BoxCollider2D collider = Background.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(5f, 5.5f);

            // Game Object
            AddButton();
            AddDisabledButton();
            AddSocialContainer();

            // Create TMP
            Title = Entry.CreateTMP("Null", "TMP_Title");
            UpdateText = Entry.CreateTMP("Null", "TMP_Update");
            Entry.SetActive(false);
        }

        private void AddSocialContainer() {
            SocialBackground = Object.Instantiate(Background);
            SocialBackground.name = "Social Link";
            SocialBackground.layer = 5;
            SocialBackground.transform.SetParent(Entry.transform);
            SocialBackground.transform.localPosition = new Vector3(3.08f, 0f, -2f);

            Object.Destroy(SocialBackground.GetComponent<PassiveButton>());
            Object.Destroy(SocialBackground.transform.Find("IgnoreClicks").gameObject);

            BoxCollider2D collider = SocialBackground.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 5.5f);

            SpriteRenderer renderer = SocialBackground.GetComponent<SpriteRenderer>();
            renderer.size = new Vector2(1f, 5.5f);
  
            SocialContainer = new GameObject { name = "SocialContainer", layer = 5 };
            SocialContainer.transform.SetParent(SocialBackground.transform);
            SocialContainer.transform.localPosition = new Vector3(0f, 0f, -2f);
            SocialBackground.SetActive(true);

            Scroller scroll = SocialBackground.AddComponent<Scroller>();
            scroll.allowX = false;
            scroll.allowY = true;
            scroll.velocity = new Vector2(0.008f, 0.005f);
            scroll.ScrollerYRange = new FloatRange(0f, 0f);
            scroll.YBounds = new FloatRange(0f, 3f);
            scroll.Inner = SocialContainer.transform;
            SocialBackground.SetActive(false);

            SpriteMask spriteMaskTemplate = Object.FindObjectsOfType<SpriteMask>().FirstOrDefault();
            GameObject SpriteMask = Object.Instantiate(spriteMaskTemplate.gameObject);
            SpriteMask.name = "Mask";
            SpriteMask.layer = 5;
            SpriteMask.transform.SetParent(SocialBackground.transform);
            SpriteMask.transform.localPosition = new Vector3(0f, 0f, 0f);
            SpriteMask.transform.localScale = new Vector3(1f, 5.1f, 1f);
            SpriteMask.SetActive(true);
        }

        private void UpdateScroll() {
            if (ModsInformation.Instance.SocialContainer == null) {
                HardelApiPlugin.Logger.LogError($"An error occurred while updating the YBounds of the ModsInformation.SocialContainer Scroll. The GameObject SocialContainer, of the ModsInformation class is not defined or at the wrong time.");
                return;
            }

            int scrollRow = Mathf.Max(ModEntries.Count - 5, 0);
            float YRange = scrollRow * 0.85f;
            MainMenuPatch.ScrollerEntries.GetComponent<Scroller>().YBounds = new FloatRange(0f, YRange);
        }

        private void AddDisabledButton() {
            GameObject template = GameObject.Find("ExitGameButton");
            if (template == null)
                return;

            EnableButton = Object.Instantiate(template, Entry.transform);
            EnableButton.transform.localPosition = new Vector3(1f, -2.15f, 0f);
            EnableButton.name = "Enable Button";
            Object.Destroy(EnableButton.GetComponent<SceneChanger>());
            Object.Destroy(EnableButton.GetComponent<AspectPosition>());
            Object.Destroy(EnableButton.GetComponent<ConditionalHide>());
            Object.Destroy(EnableButton.transform.GetChild(0).GetComponent<TextTranslatorTMP>());

            SpriteRenderer renderer = EnableButton.GetComponent<SpriteRenderer>();
            renderer.size = new Vector2(1.4f, 0.7f);

            BoxCollider2D boxCollider = EnableButton.GetComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1.4f, 0.7f);

            PassiveButton passiveButton = EnableButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            passiveButton.OnClick.AddListener((UnityAction) OnClick);

            TMP_Text text = EnableButton.transform.GetChild(0).GetComponent<TMP_Text>();
            InstanceManager.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) => {
                text.SetText("Disable");
            })));

            EnableButton.GetComponent<SpriteRenderer>().size = new Vector2(1.4f, 0.7f);
            EnableButton.GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 0.7f);

            void OnClick() {
                string CurrentPath = ModData.GetModPath();
                string PluginDirectory = Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\plugins\" + ModData.FileName;
                string DisableDirectory = Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\disable\" + ModData.FileName;

                if (ModData.IsModActive) {
                    File.Move(CurrentPath, DisableDirectory);
                    PopupMessage.PopupText("The mod has been deactivated.\nTo make the changes effective please restart Among us.", true);
                } else {
                    File.Move(CurrentPath, PluginDirectory);
                    PopupMessage.PopupText("The mod has been activated.\nTo make the changes effective please restart Among us.", true);
                }

                ModData.IsModActive = !ModData.IsModActive;
                UpdateText.GetComponent<TextMeshPro>().text = "To make the changes effective\nplease restart Among us.";
            }
        }

        private void AddButton() {
            GameObject template = GameObject.Find("ExitGameButton");
            if (template == null)
                return;

            UpdateButtonGO = Object.Instantiate(template, Entry.transform);
            UpdateButtonGO.transform.localPosition = new Vector3(-1f, -2.15f, 0f);
            UpdateButtonGO.name = "Update Button";
            Object.Destroy(UpdateButtonGO.GetComponent<SceneChanger>());
            Object.Destroy(UpdateButtonGO.GetComponent<AspectPosition>());
            Object.Destroy(UpdateButtonGO.GetComponent<ConditionalHide>());
            Object.Destroy(UpdateButtonGO.transform.GetChild(0).GetComponent<TextTranslatorTMP>());

            ButtonRolloverHandler rollover = UpdateButtonGO.GetComponent<ButtonRolloverHandler>();
            UpdateRollover = rollover;

            SpriteRenderer renderer = UpdateButtonGO.GetComponent<SpriteRenderer>();
            renderer.size = new Vector2(1.4f, 0.7f);

            BoxCollider2D boxCollider = UpdateButtonGO.GetComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1.4f, 0.7f);

            PassiveButton passiveButton = UpdateButtonGO.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            passiveButton.OnClick.AddListener((UnityAction) OnClick);

            TMP_Text text = UpdateButtonGO.transform.GetChild(0).GetComponent<TMP_Text>();
            InstanceManager.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) => {
                text.SetText("Update");
            })));

            void OnClick() {
                ModSelection.Instance?.ShowUpdateSelection(ModData, InstanceManager);
            }
        }

        private void AddSocialObject() {
            for (int i = 0; i < ModData.SocialsLink.Count; i++) {
                ModsSocial CurrentSocialLink = ModData.SocialsLink[i];
                SocialObject.Add(CurrentSocialLink.CreateObject(new Vector3(0, 2.3f - (i * 0.8f), -2f), SocialContainer));
            }

            SocialBackground.SetActive(ModData.SocialsLink.Count > 0);
        }

        public void CloseModsMenu() {
            Entry.GetComponent<TransitionOpen>().Close();
        }

        public void CloseModsMenuActive() {
            SocialObject.ForEach(social => Object.Destroy(social));
            SocialObject = new List<GameObject>();

            Entry.SetActive(false);
        }

        public void UpdateMenu() {
            if (Entry == null)
                return;

            if (Input.GetKeyUp(KeyCode.Escape))
                CloseModsMenu();

            LerpedBreath = Color.Lerp(Color.white, Color.yellow, Mathf.PingPong(Time.time, 1));
            if (UpdateButtonGO.gameObject.scene.IsValid() && ModData != null) {
                if (UpdateButtonGO.GetComponent<SpriteRenderer>() != null) {
                    UpdateButtonGO.GetComponent<SpriteRenderer>().color = ModData.CanUpdate ? Color.white : new Color(1f, 1f, 1f, 0.5f);

                    if (ModData.CanUpdate)
                        UpdateButtonGO.GetComponent<SpriteRenderer>().color = LerpedBreath;
                }
            }
        }

        internal void UpdatePosition() {
            TMP.gameObject.transform.position = new Vector3(0.025f, -0.200f, -2.000f);

            // Title
            TextMeshPro tmp = Title.GetComponent<TextMeshPro>();
            tmp.autoSizeTextContainer = true;
            tmp.fontSize = 4;
            tmp.alignment = TextAlignmentOptions.Top;

            RectTransform rect = Title.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(2f, 2.14825f);
            rect.offsetMax = new Vector2(-2f, -2.14825f);
            rect.sizeDelta = new Vector2(5.0469f, 4.2965f);
            Title.transform.localPosition = new Vector3(0f, 0.35f, -2f);

            // Update
            TextMeshPro tmpUpdate = UpdateText.GetComponent<TextMeshPro>();
            tmpUpdate.autoSizeTextContainer = true;
            tmpUpdate.fontSize = 2;
            tmpUpdate.alignment = TextAlignmentOptions.Top;

            RectTransform rectUpdate = UpdateText.GetComponent<RectTransform>();
            rectUpdate.anchorMin = new Vector2(0.5f, 0.5f);
            rectUpdate.anchorMax = new Vector2(0.5f, 0.5f);
            rectUpdate.offsetMin = new Vector2(2f, 2.14825f);
            rectUpdate.offsetMax = new Vector2(-2f, -2.14825f);
            rectUpdate.sizeDelta = new Vector2(5.0469f, 4.2965f);
            UpdateText.transform.localPosition = new Vector3(0f, -3.25f, -2f);

            // Patch Rectangle don't update the first time.
            if (!UpdatedPosition) {
                UpdateText.transform.localPosition = new Vector3(0f, -3.25f + 1.95f, -2f);
                Title.transform.localPosition = new Vector3(0f, 0.35f + 1.95f, -2f);

                UpdatedPosition = true;
            }
        }

        public void UpdateContent() {
            UpdatePosition();
            AddSocialObject();

            TMP_Text textDisableButton = EnableButton.transform.GetChild(0).GetComponent<TMP_Text>();

            string ModVersion = ModData.Version;
            if (ModVersion.IsNullOrWhiteSpace())
                ModVersion = "Version not found";

            TMP.text = $"<b>Current Version:</b> {ModVersion}\n<b>Mod Active</b>: {(ModData.IsModActive ? "Yes" : "No")}\n\n<b>Description :</b>\n{ModData.Description}";
            Title.GetComponent<TextMeshPro>().text = $"{ModData.Name}";

            // Update
            if (!ModData.IsModActive) {
                textDisableButton.text = "Enable";
                EnableButton.GetComponent<ButtonRolloverHandler>().OverColor = new Color(0f, 1f, 0f, 1f);
                EnableButton.GetComponent<ButtonRolloverHandler>().OutColor = new Color(0.105f, 0.854f, 0.423f, 1f);
                EnableButton.GetComponent<SpriteRenderer>().color = new Color(0.105f, 0.854f, 0.423f, 1f);

                UpdateText.GetComponent<TextMeshPro>().text = "This mod is currently disabled\nand cannot be updated.";
            }
            else {
                textDisableButton.text = "Disable";
                EnableButton.GetComponent<ButtonRolloverHandler>().OverColor = new Color(1f, 0f, 0f, 1f);
                EnableButton.GetComponent<ButtonRolloverHandler>().OutColor = new Color(0.901f, 0.380f, 0.360f, 1f);
                EnableButton.GetComponent<SpriteRenderer>().color = new Color(0.901f, 0.380f, 0.360f, 1f);

                UpdateText.GetComponent<TextMeshPro>().text = ModData.CanUpdate
                    ? $"Your mods is currently in version: {ModData.Version}\nA new Update is avaible: {ModData.NewTagsVersion}"
                    : $"Your mod is in lastest version !";
            }
        }

        public static void InitializeModsInformation(MainMenuManager instance, GameObject Parent) {
            if (Instance != null)
                return;

            Instance = new ModsInformation(instance, Parent);
        }

        public async Task<bool> DownloadUpdate() {
            try {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "Mod Getter");
                http.DefaultRequestHeaders.Add("Authorization", ModData.GithubToken);
                var response = await http.GetAsync(new System.Uri(ModData.UpdateLink), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null) {
                    System.Console.WriteLine("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                System.UriBuilder uri = new System.UriBuilder(codeBase);
                string fullname = System.Uri.UnescapeDataString(uri.Path);
                if (File.Exists(fullname + ".old"))
                    File.Delete(fullname + ".old");

                File.Move(fullname, fullname + ".old");

                using (var responseStream = await response.Content.ReadAsStreamAsync()) {
                    using (var fileStream = File.Create(fullname)) {
                        responseStream.CopyTo(fileStream);
                    }
                }

                return true;
            } catch (System.Exception ex) {
                HardelApiPlugin.Logger.LogError(ex.ToString());
                System.Console.WriteLine(ex);
            }
            HardelApiPlugin.Logger.LogError("Update wasn't successful\nTry again later,\nor update manually.");
            return false;
        }

        public void UpdateMods() {
            if (ModData.CanUpdate) {
                bool Updated = DownloadUpdate().GetAwaiter().GetResult();
                if (Updated)
                    UpdateText.GetComponent<TextMeshPro>().text = "The mod has been updated with success !\nRestart the game to make the changes effective.";
            }
        }
    }
}