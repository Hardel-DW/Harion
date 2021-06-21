using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harion.Utility.Utils {
    public static class ConfigEntryUtils {

        public static ConfigEntry<T> GetValue<T>(string section, string entry) {
            ConfigEntry<T> config;
            HarionPlugin.Instance.Config.TryGetEntry(section, entry, out config);

            return config;
        }
    }
}
