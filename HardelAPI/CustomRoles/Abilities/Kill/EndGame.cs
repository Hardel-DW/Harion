using HarmonyLib;

namespace HardelAPI.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class EndGameManagerPatch {
        public static void Prefix() {
            foreach (var Role in RoleManager.AllRoles) {
                KillAbility KillAbility = Role.GetAbility<KillAbility>();
                if (KillAbility == null)
                    continue;

                KillAbility.WhiteListKill = null;
            }
        }
    }
}
