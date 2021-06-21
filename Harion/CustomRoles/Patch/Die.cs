using HarmonyLib;
using Harion.Enumerations;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    class DiePatch {
        public static void Prefix(PlayerControl __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.OnPlayerDie(__instance);

                if (__instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    Role.OnLocalDie(__instance);

                if (Role.HasRole(__instance.PlayerId)) {
                    if (Role.GiveRoleAt == Moment.OnDie) { }

                    if (Role.GiveTasksAt == Moment.OnDie)
                        Role.AddImportantTasks(__instance);
                }
            }
        }
    }
}
