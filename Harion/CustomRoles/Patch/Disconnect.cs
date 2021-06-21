using HarmonyLib;
using InnerNet;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    public class Disconnect {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData data, [HarmonyArgument(1)] DisconnectReasons reason) {
            PlayerControl character = data.Character;

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started) {
                foreach (var Role in RoleManager.AllRoles) {
                    Role.OnPlayerDisconnect(character);
                }
            }
        }
    }
}
