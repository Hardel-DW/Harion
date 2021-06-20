using HarmonyLib;

namespace HardelAPI.ColorDesigner.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
    internal static class CheckColorPatch {
        private static bool isTaken(PlayerControl player, uint color) {
            foreach (GameData.PlayerInfo p in GameData.Instance.AllPlayers)
                if (!p.Disconnected && p.PlayerId != player.PlayerId && p.ColorId == color)
                    return true;
            return false;
        }

        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor) {
            uint color = bodyColor;
            if (isTaken(__instance, color) || color >= Palette.PlayerColors.Length) {
                int num = 0;
                while (num++ < 50 && (color >= ColorCreator.pickableColors || isTaken(__instance, color))) {
                    color = (color + 1) % ColorCreator.pickableColors;
                }
            }
            __instance.RpcSetColor((byte) color);
            return false;
        }
    }
}
