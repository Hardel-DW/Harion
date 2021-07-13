using Harion.ModsManagers.Patch;
using Harion.Utility.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Harion.ModsManagers.Mods {
    internal class ModEntry {

        private static List<ModEntry> ModEntries = new List<ModEntry>();
        private static bool IsOpenEntry = false;

        internal string text;
        internal GameObject Entry;
        internal GameObject Background;
        internal TextMeshPro TMP;
        internal PassiveButton button;
        internal ModManagerData ModData;
        internal MainMenuManager Instance;

        internal ModEntry(MainMenuManager instance, Vector2 position, string AssemblyName, GameObject Parent, ModManagerData data) {
            ModData = data;
            Instance = instance;

            // Game Object
            Entry = Object.Instantiate(instance.Announcement.gameObject, Parent.transform);
            Entry.name = $"Entry {data.Name}";
            Entry.transform.localPosition += (Vector3) position;

            // Text
            TMP = Entry.transform.Find("Text_TMP").gameObject.GetComponent<TextMeshPro>();
            TMP.transform.localPosition = new Vector3(0.250f, -0.365f, -2f);
            text = AssemblyName;

            // Button
            Background = Entry.transform.Find("Background").gameObject;
            PassiveButton[] buttonLeft = Background.GetComponents<PassiveButton>();
            foreach (PassiveButton passive in buttonLeft)
                Object.Destroy(passive);

            button = Background.AddComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityAction) OnClick);
            button.OnMouseOver = new UnityEvent();
            button.OnMouseOut = new UnityEvent();

            // Destroy
            Object.Destroy(Entry.GetComponent<AnnouncementPopUp>());
            Object.Destroy(Entry.GetComponent<TransitionOpen>());
            Object.Destroy(Entry.FindObject("Title_Text"));
            Object.Destroy(Entry.transform.Find("announcementsBanner").gameObject);
            Object.Destroy(Entry.transform.Find("CloseButton").gameObject);
            Object.Destroy(Background.transform.Find("IgnoreClicks").gameObject);

            // Renderer
            Background.transform.localPosition = new Vector3(0.000f, 1.300f, 1.000f);
            SpriteRenderer renderer = Background.gameObject.GetComponent<SpriteRenderer>();
            renderer.size = new Vector2(4.5f, 0.75f);
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            if (!ModData.IsModActive)
                renderer.color = new Color(0.729f, 0.309f, 0.309f, 1f);

            // Collider
            BoxCollider2D collider = Background.gameObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(4.5f, 0.75f);

            Entry.SetActive(true);
            ModEntries.Add(this);
            RefreshMaterial();
        }

        internal static void Update() {
            foreach (var entry in ModEntries) {
                entry.TMP.text = entry.text;
            }
        }

        internal static void UpdateScroll() {
            if (MainMenuPatch.ScrollerEntries == null) {
                HarionPlugin.Logger.LogError($"An error occurred while updating the YBounds of the ModManager Scroll. The GameObject ScrollerEntries, of the MainMenuPatch class is not defined or at the wrong time.");
                return;
            }

            int scrollRow = Mathf.Max(ModEntries.Count - 5, 0);
            float YRange = scrollRow * 0.85f;
            MainMenuPatch.ScrollerEntries.GetComponent<Scroller>().YBounds = new FloatRange(0f, YRange);
        }

        internal void OnClick() {
            if (IsOpenEntry == false && !ModsInformation.Instance.Entry.activeSelf) {
                IsOpenEntry = true;
                PopupMessage.PopupText("Searching for data...", false);
                ModsInformation.Instance.Entry.SetActive(true);
                TryCheckUpdate();
            }
        }

        internal void TryCheckUpdate() {
            bool Updated = CheckUpdate().GetAwaiter().GetResult();
            ModData.CanUpdate = Updated;
            if (!ModData.IsModActive)
                ModData.CanUpdate = false;

            ModsInformation.Instance.ModData = ModData;
            ModsInformation.Instance.UpdateContent();

            IsOpenEntry = false;
            PopupMessage.Close();
        }

        internal void UpdateMaterial() {
            TMP.font = ResourceLoader.FontLiberation;
            TMP.fontMaterial = ResourceLoader.Liberia;
        }

        internal async Task<bool> CheckUpdate() {
            try {
                HttpClient http = new HttpClient();

                http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue($"HarionUpdater", ModData.Version));
                if (ModData.GithubRepositoryVisibility == Configuration.GithubVisibility.Private) {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", ModData.GithubToken);
                    http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                var response = await http.GetAsync(new System.Uri(ModData.GithubApiLink()));
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null) {
                    HarionPlugin.Logger.LogWarning("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                string json = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(json);

                string tagname = data["tag_name"]?.ToString();
                if (tagname == null) {
                    HarionPlugin.Logger.LogWarning("Tag Name Not Found: " + response.StatusCode.ToString());
                    return false;
                }

                if (ModData.Version.ToLower().Equals(tagname.ToLower())) {
                    HarionPlugin.Logger.LogWarning("This mod is in Latest version: " + response.StatusCode.ToString());
                    return false;
                }

                JToken assets = data["assets"];
                if (!assets.HasValues) {
                    HarionPlugin.Logger.LogWarning("Assets Not Found: " + response.StatusCode.ToString());
                    return false;
                }

                for (JToken current = assets.First; current != null; current = current.Next) {
                    string browser_download_url = current["browser_download_url"]?.ToString();
                    if (browser_download_url != null && current["content_type"] != null) {
                        if (current["content_type"].ToString().Equals("application/x-msdownload") && browser_download_url.EndsWith(".dll")) {
                            ModData.UpdateLink = browser_download_url;
                            ModData.NewTagsVersion = tagname;
                            return true;
                        }
                    }
                }
            } catch (System.Exception ex) {
                HarionPlugin.Logger.LogError(ex.ToString());
            }

            return false;
        }

        internal static int CountModEntries => ModEntries.Count;

        internal static void RefreshMaterial() => ModEntries.ForEach(x => x.UpdateMaterial());

        internal static void RemoveDisableEntry() {
            foreach (var Entry in ModEntries.ToArray().ToList()) {
                if (!Entry.ModData.IsModActive) {
                    Object.Destroy(Entry.Entry);
                    ModEntries.Remove(Entry);
                }
            }
        }
    }
}
