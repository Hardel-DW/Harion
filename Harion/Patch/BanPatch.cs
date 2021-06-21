using HarmonyLib;
using Hazel;
using InnerNet;

namespace Harion.Patch {
    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
    public static class BanPatch {
        public static void Postfix(out bool __result) {
            __result = false;
        }
    }

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.OnDisconnect))]
    public static class OnDisconnectPatch {
        public static bool Prefix(InnerNetClient __instance, [HarmonyArgument(0)] object sender, [HarmonyArgument(1)] DisconnectedEventArgs e) {
            MessageReader message = e.Message;
            if (message != null && message.Position < message.Length)
                return false;

            return true;
        }
    }
}