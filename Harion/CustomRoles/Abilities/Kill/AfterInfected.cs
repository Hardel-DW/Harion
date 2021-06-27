using HarmonyLib;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
    public static class AfterInfected {
        public static void Postfix() {
            RoleManager Role = RoleManager.GetMainRole(PlayerControl.LocalPlayer);
            KillAbility KillAbility = Role?.GetAbility<KillAbility>();
            if (KillAbility == null)
                return;

            KillAbility.WhiteListKill = null;
        }
    }
}