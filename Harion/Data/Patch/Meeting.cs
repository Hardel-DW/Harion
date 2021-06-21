using HarmonyLib;

namespace Harion.Data.Patch {

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Awake))]
    public static class MeetingStartPatch {
        public static void Postfix(MeetingHud __instance) {
            DeadPlayer.ClearDeadPlayer();
        }
    }
}
