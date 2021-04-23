using HarmonyLib;

namespace HardelAPI.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class StartGamePatch {
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
