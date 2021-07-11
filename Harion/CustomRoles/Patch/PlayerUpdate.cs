using HarmonyLib;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class PlayerUpdatePatch {

        public static void Postfix(PlayerControl __instance) {
            if (__instance == null)
                return;

            foreach (var Role in RoleManager.AllRoles)
                if (Role != null)
                    Role.OnUpdate(__instance);
        } 
    }
}
