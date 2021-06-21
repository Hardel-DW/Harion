using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Harion.ModsManagers.Mods {
    internal static class Disable {

        internal static string[] GetDisableMod() {
            string path = Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\disable";
            Directory.CreateDirectory(path);
            string[] pluginFilesNames = Directory.GetFiles(path, "*.dll");
            if (pluginFilesNames.Count() == 0)
                HarionPlugin.Logger.LogInfo("No disable mod was found !");

            return pluginFilesNames;
        }

        internal static List<Assembly> GetDisableModData() {
            string[] paths = GetDisableMod();
            List<Assembly> assemblies = new List<Assembly>();

            foreach (var path in paths) {
                Assembly ModAssembly = Assembly.LoadFrom(path);
                HarionPlugin.Logger.LogInfo($"{ModAssembly.GetName().Name} was found !");
                assemblies.Add(ModAssembly);
            }

            return assemblies;
        }
    }
}
