using BepInEx.IL2CPP;
using HardelAPI.ModsManagers.Configuration;
using HardelAPI.ModsManagers.Mods;
using HardelAPI.Utility.Helper;
using HardelAPI.Utility.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using Object = UnityEngine.Object;
using Type = System.Type;
using System;

namespace HardelAPI.ModsManagers.Patch {

    public static class MainMenuPatch {

        public static GameObject Popup;
        public static GameObject CloseButton;
        public static GameObject Background;
        public static GameObject announcementsBanner;
        public static GameObject InnerEntries;
        public static GameObject ScrollerEntries;
        public static GameObject Mask;
        public static GameObject RefreshButton;
        public static MainMenuManager Instance;
        internal static ModSelection ModSelection = null;

        public static void OpenModsMenu() {
            Popup.SetActive(true);
            ModEntry.RefreshMaterial();
        }

        public static void CloseModsMenu() => Popup.GetComponent<TransitionOpen>().Close();

        public static void CloseModsMenuActive() => Popup.SetActive(false);

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class MainMenuStart {

            public static void Prefix(MainMenuManager __instance) {
                Instance = __instance;
                if (Popup == null)
                    CreatePopup(__instance);

                if (ModsInformation.Instance == null)
                    ModsInformation.InitializeModsInformation(__instance, Popup);

                GameObject LocalButton = GameObject.Find("PlayLocalButton");
                GameObject AccountManager = GameObject.Find("AccountManager");

                // Mods Button
                GameObject ModsButton = Object.Instantiate(LocalButton);
                if (ModsButton == null)
                    HardelApiPlugin.Logger.LogError($"ModsButton in MainMenuStart does not exist !");

                ModsButton.name = "ModsButton";
                ModsButton.transform.localPosition = new Vector3(ModsButton.transform.localPosition.x, 0f, ModsButton.transform.localPosition.z);
                Object.Destroy(ModsButton.GetComponent<ImageTranslator>());
                SpriteRenderer RendererMods = ModsButton.GetComponent<SpriteRenderer>();
                if (RendererMods != null)
                    RendererMods.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.ModsButton.png", 100f);

                PassiveButton[] buttonLeft = ModsButton.gameObject.GetComponents<PassiveButton>();
                foreach (PassiveButton passive in buttonLeft)
                    Object.Destroy(passive);

                PassiveButton ButtonPassiveLeft = ModsButton.AddComponent<PassiveButton>();
                ButtonPassiveLeft.OnClick.RemoveAllListeners();
                ButtonPassiveLeft.OnClick.AddListener((UnityAction) OpenModsMenu);
                ButtonPassiveLeft.OnMouseOver = new UnityEvent();
                ButtonPassiveLeft.OnMouseOver.AddListener((UnityAction) OnMouseOver);
                ButtonPassiveLeft.OnMouseOut = new UnityEvent();
                ButtonPassiveLeft.OnMouseOut.AddListener((UnityAction) OnMouseOut);

                void OnMouseOver() => ModsButton.GetComponent<SpriteRenderer>().color = new Color(0.3f, 1f, 0.3f, 1f);
                void OnMouseOut() => ModsButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1, 1f);

                // Credits Button
                GameObject CreditsButton = Object.Instantiate(LocalButton);
                if (CreditsButton == null)
                    HardelApiPlugin.Logger.LogError($"CreditsButton in MainMenuStart does not exist !");

                CreditsButton.name = "CreditsButton";
                CreditsButton.transform.localPosition = new Vector3(1.025f, 0f, CreditsButton.transform.localPosition.z);
                Object.Destroy(CreditsButton.GetComponent<ImageTranslator>());
                SpriteRenderer RendererCredits = CreditsButton.GetComponent<SpriteRenderer>();
                if (RendererCredits != null)
                    RendererCredits.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.CreditsButton.png", 100f);

                // AccountTab
                GameObject AccountTab = AccountManager.transform.Find("AccountTab/AccountTab").gameObject;
                AccountTab.transform.localPosition = new Vector3(AccountTab.transform.localPosition.x, 0f, AccountTab.transform.localPosition.z);
            }

            private static void CreatePopup(MainMenuManager __instance) {
                if (__instance.Announcement == null)
                    return;

                Popup = Object.Instantiate(__instance.Announcement.gameObject).DontDestroy();
                Popup.name = "ModsMenuManager";

                Object.Destroy(Popup.GetComponent<AnnouncementPopUp>());
                Object.Destroy(Popup.transform.Find("Text_TMP").gameObject);
                CloseButton = Popup.transform.Find("CloseButton").gameObject;
                Background = Popup.transform.Find("Background").gameObject;

                // announcementsBanner
                announcementsBanner = Popup.transform.Find("announcementsBanner").gameObject;
                Object.Destroy(announcementsBanner.GetComponent<ImageTranslator>());

                announcementsBanner.transform.position = new Vector3(announcementsBanner.transform.position.x, 2.2f, announcementsBanner.transform.position.z);
                announcementsBanner.GetComponent<SpriteRenderer>().sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.ModManager.png", 100f);

                SpriteRenderer renderer = Background.gameObject.GetComponent<SpriteRenderer>();
                renderer.size = new Vector2(5f, 5.5f);
                BoxCollider2D collider = Background.gameObject.GetComponent<BoxCollider2D>();
                collider.size = new Vector2(5f, 5.5f);

                PassiveButton[] buttonLeft = CloseButton.gameObject.GetComponents<PassiveButton>();
                foreach (PassiveButton passive in buttonLeft)
                    Object.Destroy(passive);

                CloseButton.transform.position = new Vector3(-2.750f, 2.5f, 0f);
                PassiveButton ButtonPassiveLeft = CloseButton.AddComponent<PassiveButton>();
                ButtonPassiveLeft.OnClick.RemoveAllListeners();
                ButtonPassiveLeft.OnClick.AddListener((UnityAction) CloseModsMenu);
                ButtonPassiveLeft.OnMouseOver = new UnityEvent();
                ButtonPassiveLeft.OnMouseOut = new UnityEvent();

                Popup.GetComponent<TransitionOpen>().OnClose.AddListener((UnityAction) CloseModsMenuActive);
                CreateScrollerEntries();
                CreateRefreshEntryButton();
            }

            private static void CreateRefreshEntryButton() {
                RefreshButton = Object.Instantiate(CloseButton, Popup.transform);
                RefreshButton.name = "RefreshButton";

                PassiveButton[] passiveButtons = RefreshButton.gameObject.GetComponents<PassiveButton>();
                foreach (PassiveButton passive in passiveButtons)
                    Object.Destroy(passive);

                RefreshButton.transform.localPosition = new Vector3(2.750f, 2.5f, -2f);
                PassiveButton passiveButton = RefreshButton.AddComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                passiveButton.OnClick.AddListener((UnityAction) OnClick);
                passiveButton.OnMouseOver = new UnityEvent();
                passiveButton.OnMouseOut = new UnityEvent();

                SpriteRenderer renderer = RefreshButton.GetComponent<SpriteRenderer>();
                renderer.sprite =  SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Refresh.png", 100f);

                void OnClick() {
                    ModEntry.RemoveDisableEntry();
                    AddDisableEntries();
                }
            }

            private static void CreateInnerEntries(GameObject Parent) {
                InnerEntries = new GameObject { name = "Entries", layer = 5 };
                InnerEntries.transform.localPosition = new Vector3(0f, 0f, -2f);
                InnerEntries.transform.localScale = new Vector3(1f, 1f, 1f);
                InnerEntries.transform.SetParent(Parent.transform);
                AddActiveEntries();
                AddDisableEntries();
            }

            private static void CreateMask(GameObject Parent) {
                // Mask
                SpriteMask spriteMaskTemplate = Object.FindObjectsOfType<SpriteMask>().FirstOrDefault();
                GameObject SpriteMask = Object.Instantiate(spriteMaskTemplate.gameObject);
                if (SpriteMask == null)
                    HardelApiPlugin.Logger.LogError($"SpriteMask in MainMenuStart does not exist !");

                SpriteMask.name = "Mask";
                SpriteMask.layer = 5;
                SpriteMask.transform.SetParent(Parent.transform);
                SpriteMask.transform.localPosition = new Vector3(0f, -0.335f, 0f);
                SpriteMask.transform.localScale = new Vector3(5f, 4.35f, 1f);
                SpriteMask.SetActive(true);
            }

            private static void CreateScrollerEntries() {
                ScrollerEntries = new GameObject { name = "ScrollerEntries", layer = 5 };
                ScrollerEntries.transform.localPosition = new Vector3(0f, 0f, 0f);
                ScrollerEntries.transform.localScale = new Vector3(1f, 1f, 1f);
                ScrollerEntries.transform.SetParent(Popup.transform);

                CreateInnerEntries(ScrollerEntries);
                CreateMask(ScrollerEntries);

                Scroller scrollEntries = ScrollerEntries.AddComponent<Scroller>();
                scrollEntries.allowX = false;
                scrollEntries.allowY = true;
                scrollEntries.velocity = new Vector2(0.008f, 0.005f);
                scrollEntries.ScrollerYRange = new FloatRange(0f, 0f);
                scrollEntries.YBounds = new FloatRange(0f, 3f);
                scrollEntries.Inner = InnerEntries.transform;
                ScrollerEntries.SetActive(true);
            }

            private static void CreateModEntrie(object MainClass, GlobalInformation Data) {
                Data.Description = PluginHelper.GetIModData<string, IModManager>(MainClass, "Description") ?? Data.Description;
                Data.Name = PluginHelper.GetIModData<string, IModManager>(MainClass, "DisplayName") ?? Data.Name;
                Data.Version = PluginHelper.GetIModData<string, IModManager>(MainClass, "Version") ?? Data.Version;
                Data.Credit = PluginHelper.GetIModData<string, IModManager>(MainClass, "Credit") ?? Data.Credit;
                Data.SmallDescription = PluginHelper.GetIModData<string, IModManager>(MainClass, "SmallDescription") ?? Data.SmallDescription;

                ModManagerData ModData = new ModManagerData(Data.Name, Data.Description, Data.SmallDescription, Data.Version, Data.Credit);
                ModData.GithubRepository = PluginHelper.GetIModData<string, IModManagerUpdater>(MainClass, "GithubRepositoryName");
                ModData.GithubAuthor = PluginHelper.GetIModData<string, IModManagerUpdater>(MainClass, "GithubAuthorName");
                ModData.GithubRepositoryVisibility = PluginHelper.GetIModData<GithubVisibility, IModManagerUpdater>(MainClass, "GithubRepositoryVisibility");
                ModData.GithubToken = PluginHelper.GetIModData<string, IModManagerUpdater>(MainClass, "GithubAccessToken");
                ModData.ModsLinks = PluginHelper.GetIModData<Dictionary<string, Sprite>, IModManagerLink>(MainClass, "ModsLinks");
                ModData.IsModActive = Data.IsActive;
                ModData.AssemblyPathDirectory = PluginHelper.AssemblyDirectory(Assembly.GetAssembly(MainClass.GetType()));
                ModData.FileName = Path.GetFileName(Assembly.GetAssembly(MainClass.GetType()).Location);
                ModData.Assembly = Assembly.GetAssembly(MainClass.GetType());
                ModData.MainClass = MainClass;
                ModData.MainTypeClass = MainClass.GetType();

                string Text = $"{Data.Name} - {Data.Version}\n{Data.SmallDescription}";
                new ModEntry(Instance, new Vector2(0f, -0.85f * ModEntry.CountModEntries), Text, InnerEntries, ModData);
                ModData.AddSocial();
            }

            private static void AddActiveEntries() {
                for (int i = 0; i < IL2CPPChainloader.Instance.Plugins.Count; i++) {
                    KeyValuePair<string, BepInEx.PluginInfo> Mod = IL2CPPChainloader.Instance.Plugins.ElementAt(i);
                    object MainClass = Mod.Value.Instance;

                    GlobalInformation Data = new GlobalInformation {
                        Description = "No Description found.",
                        Name = Mod.Value.Metadata.Name,
                        Version = $"{Mod.Value.Metadata.Version}",
                        Credit = "",
                        SmallDescription = "No Description found.",
                        IsActive = true
                    };

                    CreateModEntrie(MainClass, Data);
                }
            }

            private static void AddDisableEntries() {
                foreach (Assembly Assembly in Disable.GetDisableModData()) {
                    foreach (Type type in Assembly.GetTypes()) {
                        if (type.IsClass && type.IsSubclassOf(typeof(BasePlugin))) {
                            HardelApiPlugin.Logger.LogInfo($"{type.Name}");
                            GlobalInformation Data = new GlobalInformation {
                                Name = $"{Assembly.GetName().Name}",
                                Version = $"",
                                Credit = $"No Credits found.",
                                Description = $"No Description found.",
                                SmallDescription = $"No Description found.",
                                IsActive = false
                            };

                            object MainClass = Activator.CreateInstance(type);
                            CreateModEntrie(MainClass, Data);
                        }
                    }
                }
            }
                
            private static void ClearOldVersion() {
                try {
                    DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\plugins");
                    string[] files = directory.GetFiles("*.old").Select(file => file.FullName).ToArray();
                    foreach (var file in files)
                        File.Delete(file);
                } catch (System.Exception e) {
                    HardelApiPlugin.Logger.LogError("Exeption has been throw when clearing old version : " + e);
                    throw;
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
        public static class MainMenuUpdate  {
            public static void Prefix(MainMenuManager __instance) {
                if (Popup == null)
                    return;

                GameObject Object = __instance.gameObject.FindObject("Slider");
                if (ModSelection == null && Object != null)
                    ModSelection = new ModSelection(Popup, Object);

                if (Input.GetKeyUp(KeyCode.Escape))
                    CloseModsMenu();

                ModEntry.Update();
                if (ModsInformation.Instance != null)
                    ModsInformation.Instance.UpdateMenu();   
            }
        }
    }
}