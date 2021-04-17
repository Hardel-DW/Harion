using HarmonyLib;

namespace HardelAPI.Utility.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class PlayerUpdatePatch {

        public static void Postfix(PlayerControl __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnUpdate(__instance);
        } 
    }
}
