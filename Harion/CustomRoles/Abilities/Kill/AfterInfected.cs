using HarmonyLib;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public static class AfterInfected {
        public static void Postfix() {
            foreach (var Role in RoleManager.AllRoles) {
                KillAbility KillAbility = Role.GetAbility<KillAbility>();
                if (KillAbility == null)
                    continue;

                KillAbility.WhiteListKill = null;
            }
        }
    }
}