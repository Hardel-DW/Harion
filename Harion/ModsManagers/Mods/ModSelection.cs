using Harion.Utility.Utils;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Harion.ModsManagers.Mods {
    internal class ModSelection {

        internal List<GameObject> Button = new List<GameObject>();
        internal List<VersionUpdate> Versions = new List<VersionUpdate>();
        internal GameObject Slider;
        internal GameObject Inner;
        internal GameObject Backdrop;
        internal GameObject Mask;
        internal static ModSelection Instance = null;

        public ModSelection(GameObject Parent, GameObject Template) {
            Slider = Object.Instantiate(Template, Parent.transform);
            Slider.gameObject.name = "Mods Version Selector";
            Slider.transform.localPosition = new Vector3(0f, 0f, -50f);
            Slider.SetActive(false);
            Instance = this;

            GameObject BackdropTemplate = Template.transform.parent.parent.GetChild(0).gameObject;
            Backdrop = Object.Instantiate(BackdropTemplate, Slider.transform);
            
            PassiveButton button = Backdrop.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityAction) CloseModsMenuActive);

            SpriteRenderer Renderer = Backdrop.GetComponent<SpriteRenderer>();
            Renderer.color = new Color(0f, 0f, 0f, 0.9f);

            Slider.GetComponent<SpriteRenderer>().size = new Vector2(5.5f, 5.5f);

            Inner = Slider.gameObject.FindObject("Inner");
            Inner.transform.localPosition = new Vector3(0f, 0f, -2f);

            Scroller Scroll = Slider.GetComponent<Scroller>();
            Scroll.allowX = false;
            Scroll.allowY = true;
            Scroll.velocity = new Vector2(0.008f, 0.005f);
            Scroll.ScrollerYRange = new FloatRange(0f, 0f);
            Scroll.YBounds = new FloatRange(0f, 3f);
            Scroll.Inner = Inner.transform;

            Mask = Slider.transform.FindChild("Mask").gameObject;
            Mask.transform.localScale = new Vector3(5.25f, 5.25f, 1f);
        }

        internal void ShowUpdateSelection(ModManagerData ModData, MainMenuManager instance) {
            if (Instance == null)
                return;

            if (Button.Count > 0)
                Button.ForEach(button => Object.Destroy(button));

            Button = new List<GameObject>();
            Versions = new();

            bool success = GetAllTags(ModData).GetAwaiter().GetResult();
            if (!success)
                return;

            Slider.SetActive(true);

            foreach (VersionUpdate Version in Versions)
                CreateButton(instance, Inner, Version, ModData);

            UpdateScroll();
        }

        internal void CreateButton(MainMenuManager instance, GameObject Parent, VersionUpdate TagVersion, ModManagerData ModData) {
            // Game Object
            GameObject Entry = Object.Instantiate(instance.Announcement.gameObject, Parent.transform);
            Entry.name = $"SelectionVersion";
            Entry.transform.localPosition = new Vector3(0f, (Button.Count * -1.25f) + 0.725f, 0f);

            // Button
            GameObject Background = Entry.transform.Find("Background").gameObject;
            PassiveButton[] buttonLeft = Background.GetComponents<PassiveButton>();
            foreach (PassiveButton passive in buttonLeft)
                Object.Destroy(passive);

            PassiveButton button = Background.AddComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityAction) OnClick);
            button.OnMouseOver = new UnityEvent();
            button.OnMouseOver.AddListener((UnityAction) OnMouseOver);
            button.OnMouseOut = new UnityEvent();
            button.OnMouseOut.AddListener((UnityAction) OnMouseOut);

            // Destroy
            Object.Destroy(Entry.GetComponent<AnnouncementPopUp>());
            Object.Destroy(Entry.GetComponent<TransitionOpen>());
            Object.Destroy(Entry.transform.Find("CloseButton").gameObject);
            Object.Destroy(Entry.transform.Find("announcementsBanner").gameObject);
            Object.Destroy(Background.transform.Find("IgnoreClicks").gameObject);

            // Renderer
            Background.transform.localPosition = new Vector3(0.000f, 1.300f, 1.000f);
            SpriteRenderer renderer = Background.gameObject.GetComponent<SpriteRenderer>();
            renderer.size = new Vector2(5f, 1f);
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            renderer.color = TagVersion.ButtonColor;

            // Text
            TextMeshPro TMP = Entry.transform.Find("Text_TMP").gameObject.GetComponent<TextMeshPro>();
            TMP.transform.localPosition = new Vector3(0f, 1.3f, -2f);
            TMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
            TMP.verticalAlignment = VerticalAlignmentOptions.Middle;
            TMP.gameObject.transform.SetParent(Background.transform);

            string DisplayTest = $"Version: {TagVersion.TagName}\n<size=2>{TagVersion.NameVerion}</size>";
            instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) => {
                TMP.SetText(DisplayTest);
                TMP.fontMaterial = ResourceLoader.Liberia;
            })));

            // Collider
            BoxCollider2D collider = Background.gameObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(5f, 1f);

            Entry.SetActive(true);
            Button.Add(Entry);

            void OnClick() => ChangeVersion(TagVersion, ModData);
            void OnMouseOver() => Background.GetComponent<SpriteRenderer>().color = new Color(0.3f, 1f, 0.3f, 1f);
            void OnMouseOut() => Background.GetComponent<SpriteRenderer>().color = TagVersion.ButtonColor;
        }

        internal void ChangeVersion(VersionUpdate TagVersion, ModManagerData ModData) {
            if (TagVersion.TagName.ToLower() == ModData.Version.ToLower()) {
                PopupMessage.PopupText("This is currently the version you are using.", true);
                return;
            }

            if (Versions.ElementAt(0) == TagVersion) {
                PopupMessage.PopupUpdateMods($"You are going to update to the latest version, are you sure you want to continue ?\n{TagVersion.UrlDownload}", TagVersion.UrlDownload);
                return;
            }

            PopupMessage.PopupUpdateMods($"You are going to update this mod, are you sure you want to continue ?\n{TagVersion.UrlDownload}", TagVersion.UrlDownload);
        }

        internal void UpdateScroll() {
            if (Slider == null) {
                HarionPlugin.Logger.LogError($"An error occurred while updating the YBounds of the ModSelection.Slider Scroll. The GameObject Slider, of the ModSelection class is not defined or at the wrong time.");
                return;
            }

            int scrollRow = Mathf.Max(Button.Count - 4, 0);
            float YRange = scrollRow * 1.25f;
            Slider.GetComponent<Scroller>().YBounds = new FloatRange(0f, YRange);
        }

        internal async Task<bool> GetAllTags(ModManagerData ModData) {
            try {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "Mod Getter");
                var response = await http.GetAsync(new Uri(ModData.GithubTag()), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null) {
                    HarionPlugin.Logger.LogWarning("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                string json = await response.Content.ReadAsStringAsync();
                JArray data = JArray.Parse(json);

                foreach (JToken tagVersion in data) {
                    string tagname = tagVersion["tag_name"]?.ToString();
                    string versionName = tagVersion["name"]?.ToString();
                    string body = tagVersion["body"]?.ToString();

                    if (tagname == null) {
                        HarionPlugin.Logger.LogWarning("Tag Name Not Found: " + response.StatusCode.ToString());
                        return false;
                    }

                    if (body == null) {
                        HarionPlugin.Logger.LogWarning("body Not Found: " + response.StatusCode.ToString());
                        return false;
                    }

                    if (versionName == null) {
                        HarionPlugin.Logger.LogWarning("Name of version Not Found: " + response.StatusCode.ToString());
                        return false;
                    }

                    JToken assets = tagVersion["assets"];
                    if (!assets.HasValues) {
                        HarionPlugin.Logger.LogWarning("Assets Not Found: " + response.StatusCode.ToString());
                        return false;
                    }

                    for (JToken current = assets.First; current != null; current = current.Next) {
                        string browser_download_url = current["browser_download_url"]?.ToString();
                        string created_at = current["created_at"]?.ToString();

                        if (browser_download_url != null && current["content_type"] != null) {
                            if (current["content_type"].ToString().Equals("application/x-msdownload") && browser_download_url.EndsWith(".dll")) {
                                VersionUpdate VersionUpdate = new VersionUpdate(tagname, versionName, browser_download_url);
                                VersionUpdate.ReleaseDate = DateTime.Parse(created_at);
                                VersionUpdate.Markdown = body;

                                Color Button = Color.white;
                                if (ModData.Version.ToLower() == tagname.ToLower())
                                    Button = new Color(0.070f, 0.929f, 0.643f, 1f);

                                if (Versions.Count == 0)
                                    Button = Color.yellow;

                                VersionUpdate.ButtonColor = Button;
                                Versions.Add(VersionUpdate);
                            }
                        }
                    }
                }

                return true;
            } catch (Exception ex) {
                HarionPlugin.Logger.LogError(ex.ToString());
            }

            return false;
        }

        public void CloseModsMenuActive() => Slider.SetActive(false);
    }
}
