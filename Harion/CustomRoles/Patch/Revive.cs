using HarmonyLib;
using Harion.Enumerations;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
    class RevivePatch {
        public static void Prefix(PlayerControl __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.OnPlayerRevive(__instance);

                if (__instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    Role.OnLocalRevive(__instance);

                if (Role.HasRole(__instance.PlayerId)) {
                    if (Role.GiveRoleAt == Moment.OnRevive) { }

                    if (Role.GiveTasksAt == Moment.OnRevive)
                        Role.AddImportantTasks(__instance);
                }
            }
        }
    }
}
