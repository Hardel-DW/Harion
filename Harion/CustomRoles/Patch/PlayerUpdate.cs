using Harion.Utility.Utils;
using HarmonyLib;

namespace Harion.CustomRoles.Patch {

    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerUpdatePatchs {

        [HarmonyPostfix]
        public static void Test(PlayerPhysics __instance) {
            if (__instance.myPlayer == null || !GameUtils.GameStarted || PlayerControlUtils.IsPlayerNull || RoleManager.AllRoles == null || RoleManager.AllRoles.Count == 0)
                return;

            foreach (RoleManager Role in RoleManager.AllRoles) {
                Role.OnUpdate(__instance.myPlayer);
                Role.DefineVisibleByWhitelist();
            }
        }
    }
}