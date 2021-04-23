﻿using HarmonyLib;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class EndGamePatch {
        public static void Prefix(AmongUsClient __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.ClearRole();
                Role.OnGameEnded();
            }
        }
    }
}
