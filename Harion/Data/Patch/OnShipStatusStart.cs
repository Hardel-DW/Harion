using HarmonyLib;

namespace Harion.Data.Patch {

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class MeetingEndPatch {
        public static void Postfix() {
            DeadPlayer.ClearDeadPlayer();
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static class OnSpawnPlayer {
        public static void Postfix() {
            foreach (var Player in PlayerControl.AllPlayerControls) {
                new InitialPlayerApparence(Player);
            }
        }
    }
}