using HarmonyLib;
using UnityEngine;

namespace Harion.CustomRoles.Patch {

    internal static class TaskPatches {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private class RecomputeTaskPatch {
            private static bool Prefix(GameData __instance) {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;

                foreach (GameData.PlayerInfo playerInfo in __instance.AllPlayers) {
                    bool ValidPlayer = true;

                    if (playerInfo.Disconnected)
                        ValidPlayer = false;

                    if (playerInfo.Tasks == null)
                        ValidPlayer = false;

                    if (!playerInfo.Object)
                        ValidPlayer = false;

                    if (!PlayerControl.GameOptions.GhostsDoTasks && playerInfo.IsDead)
                        ValidPlayer = false;

                    if (playerInfo.IsImpostor)
                        ValidPlayer = false;

                    // Custom Role
                    RoleManager MainRole = RoleManager.GetMainRole(playerInfo.Object);
                    if (MainRole != null)
                        if (!MainRole.HasTask)
                            ValidPlayer = false;
                    
                    if (ValidPlayer) {
                        foreach (var task in playerInfo.Tasks) {
                            __instance.TotalTasks++;

                            if (task.Complete)
                                __instance.CompletedTasks++;
                        }
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        private class ConsoleCanUsePatch {
            private static bool Prefix(ref float __result, Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse) {
                float num = float.MaxValue;
                PlayerControl player = pc.Object;
                Vector2 truePosition = player.GetTruePosition();
                Vector3 position = __instance.transform.position;
                couldUse = true;

                RoleManager role = RoleManager.GetMainRole(player);
                if (role != null)
                    if (!role.HasTask)
                        couldUse = false;

                if (pc.IsDead && (!PlayerControl.GameOptions.GhostsDoTasks || __instance.GhostsIgnored))
                    couldUse = false;

                if (!player.CanMove)
                    couldUse = false;

                if (!__instance.AllowImpostor && pc.IsImpostor)
                    couldUse = false;

                if (__instance.onlySameRoom && !__instance.InRoom(truePosition))
                    couldUse = false;

                if (__instance.onlyFromBelow && truePosition.y > position.y)
                    couldUse = false;

                if (!__instance.FindTask(player))
                    couldUse = false;

                canUse = couldUse;
                if (canUse) {
                    num = Vector2.Distance(truePosition, __instance.transform.position);
                    canUse &= (num <= __instance.UsableDistance);
                    if (__instance.checkWalls) {
                        canUse &= !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShadowMask, false);
                    }
                }

                __result = num;
                return false;
            }
        }
    }
}
