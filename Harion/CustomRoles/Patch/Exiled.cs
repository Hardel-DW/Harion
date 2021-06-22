using HarmonyLib;
using System.Linq;

namespace Harion.CustomRoles.Patch {

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
            RoleManager Role = RoleManager.GetMainRole(exiled.Object);
            if (Role != null && PlayerControl.GameOptions != null) {
                if (PlayerControl.GameOptions.ConfirmImpostor || Role.ForceExiledReveal) {
                    __instance.completeString = $"{exiled.PlayerName} was the {Role.Name}";
                }
            }
        }
    }
}
