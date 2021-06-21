using BepInEx;
using BepInEx.IL2CPP;
using Harion.ModsManagers.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Harion.Utility.Helper {
    public static class PluginHelper {

        /// <summary>
        /// Gets the "Id" string provided to the first derivative class of <see cref="BasePlugin"/> with the attribute <see cref="BepInPlugin"/> in the current call stack.
        /// </summary>
        /// <returns>A plugin id or <see cref="string.Empty"/></returns>
        public static string GetCallingPluginId(int frameIndex = 3) {
            StackTrace stackTrace = new StackTrace(frameIndex);
            for (int i = 0; i < stackTrace.GetFrames().Length; i++) {
                MethodBase method = stackTrace.GetFrame(i).GetMethod();
                Type type = method.ReflectedType;

                if (!type.IsClass || !type.IsSubclassOf(typeof(BasePlugin)) || type.IsAbstract)
                    continue;

                foreach (CustomAttributeData attribute in type.CustomAttributes) {
                    if (attribute.AttributeType != typeof(BepInPlugin))
                        continue;

                    CustomAttributeTypedArgument arg = attribute.ConstructorArguments.FirstOrDefault();
                    if (arg == null || arg.ArgumentType != typeof(string) || arg.Value is not string value)
                        continue;

                    return value;
                }
            }

            return string.Empty;
        }

        public static T GetIModData<T, U>(object Mod, string property) {
            if (Mod == null || property.IsNullOrWhiteSpace())
                return default(T);

            if (!typeof(IModManager).IsAssignableFrom(Mod.GetType()))
                return default(T);

            if (!typeof(U).IsAssignableFrom(Mod.GetType()))
                return default(T);

            PropertyInfo Props = Mod.GetType().GetProperty(property);
            if (typeof(T) != Props.PropertyType)
                return default(T);

            return (T) Props.GetValue(Mod, null);
        }

        public static string AssemblyDirectory(Assembly assembly) {
            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static bool ModHasInterface<T>(object Mod) => typeof(T).IsAssignableFrom(Mod.GetType());
    }
}
