using HarmonyLib;
using HardelAPI.Utility.Enumerations;
using UnityEngine;

namespace HardelAPI.Utility.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__78), nameof(PlayerControl._CoSetTasks_d__78.MoveNext))]
    public static class TasksPatch {
        public static void Postfix(PlayerControl._CoSetTasks_d__78 __instance) {
            if (__instance == null)
                return;

            foreach (var Role in RoleManager.AllRoles)
                if (Role.GiveTasksAt == Moment.StartGame && Role.HasRole(__instance.__this) && PlayerControl.LocalPlayer.PlayerId == __instance.__this.PlayerId)
                    Role.AddImportantTasks(__instance.__this);
        }
    }
}
