
using System.Linq;
using HarmonyLib;

namespace Harion.Reactor.Patch {
    internal static class PlayerIdPatch {
        [HarmonyPatch(typeof(GameData), nameof(GameData.GetAvailableId))]
        public static class GetAvailableIdPatch {
            public static bool Prefix(GameData __instance, out sbyte __result) {
                sbyte i = 0;

                while (__instance.AllPlayers.ToArray().Any(p => p.PlayerId == i)) {
                    i++;
                }

                __result = i;

                return false;
            }
        }
    }
}