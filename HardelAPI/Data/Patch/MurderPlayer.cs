using HarmonyLib;
using System;

namespace HardelAPI.Data.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class MurderPlayer {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target) {
            new DeadPlayer(target, DateTime.UtcNow, DeathReason.Kill, __instance);
        }
    }
}
