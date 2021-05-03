using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace HardelAPI.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipStatusStart {
        public static void Postfix(ShipStatus __instance) {
            Utility.HelperSprite.HerePoint = null;
        }
    }
}
