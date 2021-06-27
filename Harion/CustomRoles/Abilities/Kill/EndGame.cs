using HarmonyLib;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class EndGameManagerPatch {
        public static void Prefix() {
            RoleManager Role = RoleManager.GetMainRole(PlayerControl.LocalPlayer);
            KillAbility KillAbility = Role?.GetAbility<KillAbility>();
            if (KillAbility == null)
                return;

            KillAbility.WhiteListKill = null;
        }
    }
}
