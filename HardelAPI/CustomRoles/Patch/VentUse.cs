using HarmonyLib;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
    public static class VentExit {
        public static void Postfix(Vent __instance, [HarmonyArgument(0)] PlayerControl Player) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnExitVent(__instance, Player);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
    public static class VentEnter {
        public static void Postfix(Vent __instance, [HarmonyArgument(0)] PlayerControl Player) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnEnterVent(__instance, Player);
        }
    }
}
