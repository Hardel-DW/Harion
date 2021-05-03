using HarmonyLib;

namespace HardelAPI.Data.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipStatusStart {
        public static void Postfix(ShipStatus __instance) {
            DeadPlayer.ClearDeadPlayer();
        }
    }
}
