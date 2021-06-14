using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HardelAPI.CustomOptions;
using HardelAPI.CustomOptions.UI;
using HardelAPI.ModsManagers.Configuration;
using HardelAPI.ModsManagers.Mods;
using HardelAPI.Reactor;
using HardelAPI.Reactor.Patch;
using HardelAPI.ServerManagers.Controls;
using HardelAPI.ServerManagers.Patch;
using HardelAPI.Utility.Helper;
using HardelAPI.Utility.Utils;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI {

    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    public class HardelApiPlugin : BasePlugin, IModManager, IModManagerUpdater, IModManagerLink {
        public const string Id = "fr.hardel.api";
        public static ManualLogSource Logger;

        public static CustomOptionHeader HardelApiHeader;
        public static CustomToggleOption ShowRoleInName;
        public static CustomToggleOption DeadSeeAllRoles;
        private GameObject gameObject;

        internal static ConfigFile ConfigFile { get; set; }
        public static ConfigEntry<bool> StreamerMode;
        public static ConfigEntry<bool> ShowOfficialRegions;
        public static ConfigEntry<bool> ShowExtraRegions;

        public static HardelApiPlugin Instance => PluginSingleton<HardelApiPlugin>.Instance;

        public Harmony Harmony => new Harmony(Id);

        // Mod Manager Information
        public string DisplayName => "Harion";
        public string Version => "V1.0";
        public string Description => "Harion is a mod API adding compatibility between mods, and various functionality, and ensures the smooth running of modding.\nAdding a ModManager, key configuration, server management, role harmonization, game options, colors and more...";
        public string Credit => "Developer: Hardel";
        public string SmallDescription => "Harion is an Framework.";

        // Mod Manager Github Auto Updater
        public string GithubRepositoryName => "HardelAPI";
        public string GithubAuthorName => "Hardel-DW";
        public GithubVisibility GithubRepositoryVisibility => GithubVisibility.Public;
        public string GithubAccessToken => "";

        public Dictionary<string, Sprite> ModsLinks => new Dictionary<string, Sprite>() {
            { "https://www.patreon.com/hardel", ModsSocial.PatreonSprite },
            { "https://discord.gg/HZtCDK3s",  ModsSocial.DiscordSprite }
        };

        public HardelApiPlugin() {
            PluginSingleton<BasePlugin>.Initialize();
            RegisterInIl2CppAttribute.Initialize();

            ChainloaderHooks.OnPluginLoad(this);
        }

        public override void Load() {
            // PatchAll
            Logger = Log;
            Harmony.PatchAll();

            // Harion GameObject
            gameObject = new GameObject(nameof(HardelApiPlugin)).DontDestroy();
            gameObject.AddComponent<HarionComponent>().Plugin = this;
            gameObject.AddComponent<Coroutines.Component>();


            // Initialiaze Thing
            ColorHelper.Load();
            HarionVersionShower.Initialize();
            SplashSkip.Initialize();
            HudPosition.Load();
            Button.InitializeBaseButton();
            RegionsEditor.SetUp();
            ResourceLoader.LoadAssets();

            // Config Bind
            StreamerMode = Config.Bind("Preferences", "Enable Streamer Mode", false);
            ShowOfficialRegions = Config.Bind("Preferences", "Show Official Regions", true, "If the official regions should be shown when displaying the regions menu");
            ShowExtraRegions = Config.Bind("Preferences", "Show Extra Regions", true, "If the extra regions added by default in Unify should be shown when displaying the regions menu");

            // Game Options
            HardelApiHeader = CustomOption.AddHeader("<b>Hardel API Option :</b>");
            ShowRoleInName = CustomOption.AddToggle("Show role in name", false);
            DeadSeeAllRoles = CustomOption.AddToggle("Dead see player role", false);
            HardelApiHeader.HudStringFormat = (option, name, value) => $"\n{name}";
        }

        public override bool Unload() {
            Harmony.UnpatchSelf();
            gameObject.Destroy();

            return base.Unload();
        }
    }
}
