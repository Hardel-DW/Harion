using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static class ShipStatusStart {
        public static void Postfix(ShipStatus __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.HasWin = false;
                RoleManager.WinPlayer = new List<PlayerControl>();
                RoleManager.SpecificNameInformation = new Dictionary<PlayerControl, (Color color, string name)>();
                HudUpdatePatch.MeetingIsPassed = false;
                Role.OnGameStarted();
            }
        }
    }
}
