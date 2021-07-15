using HarmonyLib;
using System.Collections.Generic;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static class ShipStatusStart {
        public static void Postfix(ShipStatus __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.HasWin = false;
                RoleManager.WinPlayer = new();
                RoleManager.SpecificNameInformation = new();
                HudUpdatePatch.MeetingIsPassed = false;
                Role.OnGameStarted();
            }
        }
    }
}
