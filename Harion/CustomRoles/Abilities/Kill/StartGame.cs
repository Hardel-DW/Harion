using HarmonyLib;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static class StartGamePatch {
        public static void Prefix() {
            RoleManager Role = RoleManager.GetMainRole(PlayerControl.LocalPlayer);
            KillAbility KillAbility = Role?.GetAbility<KillAbility>();
            if (KillAbility == null)
                return;

            KillAbility.WhiteListKill = null;
        }
    }
}
