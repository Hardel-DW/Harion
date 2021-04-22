using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using Essentials.Options;
using HarmonyLib;
using Reactor;

namespace HardelAPI {

    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class Plugin : BasePlugin {
        public const string Id = "fr.hardel.api";
        public static ManualLogSource Logger;

        public static CustomToggleOption ShowRoleInName = CustomOption.AddToggle("Show role in name", false);
        public static CustomToggleOption DeadSeeAllRoles = CustomOption.AddToggle("Dead see player role", false);

        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; private set; }

        public override void Load() {
            Logger = Log;
            RegisterInIl2CppAttribute.Register();
            Harmony.PatchAll();
        }
    }
}
