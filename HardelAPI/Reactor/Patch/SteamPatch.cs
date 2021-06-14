/** This is Reactor Source Code, from js6pak.
 * If Reactor asks me, I will deleted this file.
 * 
 * Reactor Github :
 * https://github.com/NuclearPowered/Reactor/tree/master/Reactor
 * 
 * Link to Orignal Class SteamPatch :
 * https://github.com/NuclearPowered/Reactor/blob/master/Reactor/Patches/SteamPatch.cs
 * 
 * Discord :
 * https://discord.gg/et5XGTMfPz
 * 
 * Website :
 * https://reactor.gg/
 * 
 * Documentation :
 * https://docs.reactor.gg/
*/

using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace Reactor.Patches {
    internal static class SteamPatch {
        [HarmonyPatch]
        public static class RestartAppIfNecessaryPatch {
            public const string TypeName = "Steamworks.SteamAPI, Assembly-CSharp-firstpass";
            public const string MethodName = "RestartAppIfNecessary";

            public static bool Prepare() {
                return Type.GetType(TypeName, false) != null;
            }

            public static MethodBase TargetMethod() {
                return AccessTools.Method(TypeName + ":" + MethodName);
            }

            public static bool Prefix(out bool __result) {
                const string file = "steam_appid.txt";

                if (!File.Exists(file)) {
                    File.WriteAllText(file, "945360");
                }

                return __result = false;
            }
        }
    }
}