using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HardelAPI.Utility.CustomRoles;
using HarmonyLib;
using Reactor;

namespace HardelAPI {

    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class Plugin : BasePlugin {
        public const string Id = "fr.hardel.api";
        public static ManualLogSource Logger;

        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; private set; }

        public override void Load() {
            Logger = Log;
            Harmony.PatchAll();
            RegisterInCustomRolesAttribute.Register();
        }
    }
}
