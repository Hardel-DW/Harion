using HarmonyLib;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    static class MurderPlayer {
        static void Postfix(ref PlayerControl __instance, [HarmonyArgument(0)] ref PlayerControl target) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnMurderKill(__instance, target);
        }
    }
}
