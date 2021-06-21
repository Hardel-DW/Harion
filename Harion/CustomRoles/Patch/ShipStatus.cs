using HarmonyLib;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipStatusPatch {
        public static void Postfix(ShipStatus __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnShipStatusStart(__instance);
        }
    }
}
