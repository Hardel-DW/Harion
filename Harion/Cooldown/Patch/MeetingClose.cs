using HarmonyLib;

namespace Harion.Cooldown.Patch {
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class MeetingClosePatch {
        public static void Postfix() {
            CooldownButton.UsableButton = true;
            foreach (var button in CooldownButton.RegisteredButtons) {
                button.Timer = button.MaxTimer;
            }
        }
    }
}
