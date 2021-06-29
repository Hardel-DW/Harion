using Harion.ModsManagers;
using Harion.ModsManagers.Configuration;
using Harion.ModsManagers.Mods;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Harion {
    public class PublicModData : ModRegistry, IModManager, IModManagerUpdater, IModManagerLink {
        public string DisplayName => "Harion";
        public string Version => "V" + typeof(HarionPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        public string Description => "Harion is a mod API adding compatibility between mods, and various functionality, and ensures the smooth running of modding.\nAdding a ModManager, key configuration, server management, role harmonization, game options, colors and more...";
        public string Credit => "Developer: Hardel";
        public string SmallDescription => "Harion is an Framework.";

        // Mod Manager Github Auto Updater
        public string GithubRepositoryName => "Harion";
        public string GithubAuthorName => "Hardel-DW";
        public GithubVisibility GithubRepositoryVisibility => GithubVisibility.Public;
        public string GithubAccessToken => "";

        public Dictionary<string, Sprite> ModsLinks => new Dictionary<string, Sprite>() {
            { "https://www.patreon.com/hardel", ModsSocial.PatreonSprite },
            { "https://discord.gg/HZtCDK3s",  ModsSocial.DiscordSprite }
        };
    }
}
