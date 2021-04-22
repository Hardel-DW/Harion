﻿using HarmonyLib;

namespace HardelAPI.Utility.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipStatusStart {
        public static void Postfix(ShipStatus __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                if (Role.TaskAreRemove)
                    Role.TaskAreRemove = false;

                HudUpdatePatch.MeetingIsPassed = false;
                Role.WhiteListKill = null;
                Role.OnGameStart();
            }
        }
    }
}
