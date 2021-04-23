using HarmonyLib;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.GenericShow))]
    public static class OpenNormalMapBehaviourPatch {
        public static void Postfix(MapBehaviour __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnMinimapOpen(PlayerControl.LocalPlayer, __instance);
        } 
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
    public static class CloseMapBehaviourPatch {
        public static void Postfix(MapBehaviour __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnMinimapClose(PlayerControl.LocalPlayer, __instance);
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    public static class UpdateMapBehaviourPatch {
        public static void Postfix(MapBehaviour __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnMinimapUpdate(PlayerControl.LocalPlayer, __instance);
        }
    }
}
