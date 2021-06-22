using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using Harion.ColorDesigner;
using Harion.CustomOptions;
using Harion.CustomOptions.UI;
using Harion.Patch;
using Harion.Reactor;
using Harion.Reactor.Patch;
using Harion.ServerManagers.Controls;
using Harion.ServerManagers.Patch;
using Harion.Utility.Utils;
using HarmonyLib;
using UnityEngine;

namespace Harion {

    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    public class HarionPlugin : BasePlugin {
        public const string Id = "fr.hardel.api";
        public static ManualLogSource Logger;

        public static CustomOptionHolder HarionHeader;
        public static CustomToggleOption ShowRoleInName;
        public static CustomToggleOption DeadSeeAllRoles;
        private GameObject gameObject;

        internal static ConfigFile ConfigFile { get; set; }
        public static ConfigEntry<bool> StreamerMode;
        public static ConfigEntry<bool> ShowOfficialRegions;
        public static ConfigEntry<bool> ShowExtraRegions;

        public static HarionPlugin Instance => PluginSingleton<HarionPlugin>.Instance;

        public Harmony Harmony => new Harmony(Id);

        public override void Load() {
            Logger = Log;
            RegisterInIl2CppAttribute.Register();

            // Harion GameObject
            gameObject = new GameObject(nameof(HarionPlugin)).DontDestroy();
            gameObject.AddComponent<HarionComponent>().Plugin = this;
            gameObject.AddComponent<Coroutines.Component>();

            // PatchAll
            PluginSingleton<HarionPlugin>.Instance = this;
            Harmony.PatchAll();

            // Initialiaze Thing
            ColorCreator.Load();
            HarionVersionShower.Initialize();
            SplashSkip.Initialize();
            HudPosition.Load();
            Button.InitializeBaseButton();
            RegionsEditor.SetUp();
            ResourceLoader.LoadAssets();
            ChangeNamePatch.Initialize();

            // Config Bind
            StreamerMode = Config.Bind("Preferences", "Enable Streamer Mode", false);
            ShowOfficialRegions = Config.Bind("Preferences", "Show Official Regions", true, "If the official regions should be shown when displaying the regions menu");
            ShowExtraRegions = Config.Bind("Preferences", "Show Extra Regions", true, "If the extra regions added by default in Unify should be shown when displaying the regions menu");

            // Game Options
            HarionHeader = CustomOption.AddHolder("<b>Harion Option :</b>");
            ShowRoleInName = CustomOption.AddToggle("Show role in name", false, HarionHeader);
            DeadSeeAllRoles = CustomOption.AddToggle("Dead see player role", false, HarionHeader);
            HarionHeader.HudStringFormat = (option, name, value) => $"\n{name}";
        }

        public override bool Unload() {
            Harmony.UnpatchSelf();
            gameObject.Destroy();

            return base.Unload();
        }
    }
}
