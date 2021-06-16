using HardelAPI.Utility.Utils;
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

namespace HardelAPI.ModsManagers.Mods {
    internal class ModSelection {

        internal List<GameObject> Button = new List<GameObject>();
        internal Dictionary<string, string> Tags = new Dictionary<string, string>();
        internal GameObject Slider;
        internal static ModSelection Instance = null;

        public ModSelection(GameObject Parent, GameObject Template) {
            Slider = Object.Instantiate(Template, Parent.transform);
            Slider.gameObject.name = "Mods Version Selector";
            Slider.transform.localPosition = new Vector3(0f, 0f, -50f);
            Slider.SetActive(false);
            Instance = this;
        }

        internal void ShowUpdateSelection(ModManagerData ModData, MainMenuManager instance) {
            if (Instance == null)
                return;

            Button = new List<GameObject>();
            Tags = new Dictionary<string, string>();

            bool success = GetAllTags(ModData).GetAwaiter().GetResult();
            if (!success)
                return;

            Slider.SetActive(true);

            GameObject Parent = Slider.transform.Find("Inner").gameObject;
            foreach (var Tag in Tags) {
                HardelApiPlugin.Logger.LogInfo(Tag.Key);
                CreateButton(instance, Parent, Tag, ModData);
            }
        }

        internal void CreateButton(MainMenuManager instance, GameObject Parent, KeyValuePair<string, string> Tag, ModManagerData ModData) {
            // Game Object
            GameObject Entry = Object.Instantiate(instance.Announcement.gameObject, Parent.transform);
            Entry.name = $"SelectionVersion";
            Entry.transform.localPosition = new Vector3(0f, 0f, 0f);

            // Text
            TextMeshPro TMP = Entry.transform.GetChild(0).GetComponent<TextMeshPro>();
            TMP.transform.localPosition = new Vector3(0.250f, -0.365f, -2f);
            TMP.fontMaterial = ResourceLoader.Liberia;

            TMP_Text text = Entry.transform.GetChild(0).GetComponent<TMP_Text>();
            instance.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) => text.SetText(Tag.Key))));

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
            renderer.size = new Vector2(4.5f, 0.75f);
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            if (ModData.Version.ToLower().Equals(Tag.Key.ToLower()))
                renderer.color = new Color(0.729f, 0.729f, 0.309f, 1f);

            // Collider
            BoxCollider2D collider = Background.gameObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(4.5f, 0.75f);

            Entry.SetActive(true);
            Button.Add(Entry);

            void OnClick() => PopupMessage.PopupLink($"Are you sure you want to continue on the following link?\n{Tag.Value}", Tag.Value);
            void OnMouseOver() => Background.GetComponent<SpriteRenderer>().color = new Color(0.3f, 1f, 0.3f, 1f);
            void OnMouseOut() => Background.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1, 1f);
        }

        internal async Task<bool> GetAllTags(ModManagerData ModData) {
            try {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "Mod Getter");
                var response = await http.GetAsync(new System.Uri(ModData.GithubTag()), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null) {
                    HardelApiPlugin.Logger.LogWarning("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                string json = await response.Content.ReadAsStringAsync();
                JArray data = JArray.Parse(json);

                foreach (JToken tagVersion in data) {
                    string tagname = tagVersion["tag_name"]?.ToString();
                    if (tagname == null) {
                        HardelApiPlugin.Logger.LogWarning("Tag Name Not Found: " + response.StatusCode.ToString());
                        return false;
                    }

                    JToken assets = tagVersion["assets"];
                    if (!assets.HasValues) {
                        HardelApiPlugin.Logger.LogWarning("Assets Not Found: " + response.StatusCode.ToString());
                        return false;
                    }

                    for (JToken current = assets.First; current != null; current = current.Next) {
                        string browser_download_url = current["browser_download_url"]?.ToString();
                        if (browser_download_url != null && current["content_type"] != null) {
                            if (current["content_type"].ToString().Equals("application/x-msdownload") && browser_download_url.EndsWith(".dll")) {
                                Tags.Add(tagname, browser_download_url);
                            }
                        }
                    }
                }

                return true;
            } catch (Exception ex) {
                HardelApiPlugin.Logger.LogError(ex.ToString());
            }

            return false;
        }
    }
}
