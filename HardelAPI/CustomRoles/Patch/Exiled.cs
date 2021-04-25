using HarmonyLib;
using System.Linq;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static class Exiled {
        public static void Prefix(PlayerControl __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnExiledPlayer(__instance);
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    public static class ExileControllerPatch {
        public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo exiled, ExileController __instance) {
            if (RoleManager.GetAllRoles(exiled.Object).Count > 0) {
                RoleManager Role = RoleManager.GetMainRole(exiled.Object);
                if (PlayerControl.GameOptions.ConfirmImpostor || Role.ForceExiledReveal) {
                    __instance.completeString = $"{exiled.PlayerName} was the {Role.Name}";
                }
            }
        }
    }
}
