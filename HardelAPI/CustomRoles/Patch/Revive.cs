using HarmonyLib;
using HardelAPI.Utility.Enumerations;
using UnityEngine;

namespace HardelAPI.Utility.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
    class RevivePatch {
        public static void Prefix(PlayerControl __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.OnRevive(__instance);

                if (Role.HasRole(__instance.PlayerId)) {
                    if (Role.GiveRoleAt == Moment.OnRevive) { }

                    if (Role.GiveTasksAt == Moment.OnDie)
                        Role.AddImportantTasks(__instance);
                }
            }
        }
    }
}
