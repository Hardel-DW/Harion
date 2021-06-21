using HarmonyLib;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class ExitGamePatch {
        public static void Prefix(AmongUsClient __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                KillAbility KillAbility = Role.GetAbility<KillAbility>();
                if (KillAbility != null)
                    KillAbility.WhiteListKill = null;
            }
        }
    }
}
