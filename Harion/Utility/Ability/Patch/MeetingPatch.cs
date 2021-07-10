using HarmonyLib;

namespace Harion.Utility.Ability.Patch {

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Awake))]
    public static class MeetingStartPatch {
        public static void Postfix(MeetingHud __instance) {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                if (Morphing.IsMorphed(player)) {
                    Morphing.Unmorph(player, true);
                }

                if (Invisbility.IsInvisible(player)) {
                    Invisbility.StopInvisibility(player);
                }
            }
        }
    }
}
