using HarmonyLib;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
    public static class EndCriteria {
        public static void Postfix() {
            foreach (var Role in RoleManager.AllRoles) {
                bool endCrieria = Role.WinCriteria();

                if (endCrieria) {
                    Role.HasWin = true;
                    Role.ForceEndGame();
                }
            }
        }
    }
}
