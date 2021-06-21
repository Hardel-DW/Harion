using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipStatusStart {
        public static void Postfix(ShipStatus __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                if (Role.TaskAreRemove)
                    Role.TaskAreRemove = false;

                Role.HasWin = false;
                RoleManager.WinPlayer = new List<PlayerControl>();
                RoleManager.specificNameInformation = new Dictionary<PlayerControl, (Color color, string name)>();
                HudUpdatePatch.MeetingIsPassed = false;
                Role.OnGameStarted();
            }
        }
    }
}
