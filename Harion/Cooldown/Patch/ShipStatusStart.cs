using HarmonyLib;

namespace Harion.Cooldown.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class StartPatch {
        public static void Prefix(ShipStatus __instance) {
            CooldownButton.UsableButton = true;
        }
    }
}
