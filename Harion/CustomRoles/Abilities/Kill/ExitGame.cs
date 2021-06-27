using HarmonyLib;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class ExitGamePatch {
        public static void Prefix(AmongUsClient __instance) {
            RoleManager Role = RoleManager.GetMainRole(PlayerControl.LocalPlayer);
            KillAbility KillAbility = Role?.GetAbility<KillAbility>();
            if (KillAbility == null)
                return;

            KillAbility.WhiteListKill = null;
        }
    }
}
