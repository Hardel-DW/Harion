using Harion.Utility.Helper;
using HarmonyLib;

namespace Harion.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipStatusStart {
        public static void Postfix(ShipStatus __instance) {
            SpriteHelper.HerePoint = null;
        }
    }
}
