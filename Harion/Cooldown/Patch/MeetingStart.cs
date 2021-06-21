using HarmonyLib;

namespace Harion.Cooldown.Patch {
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class ButtonResetPatch {
        public static void Postfix(MeetingHud __instance) {
            CooldownButton.UsableButton = false;
            for (int i = 0; i < CooldownButton.RegisteredButtons.Count; i++) {
                if (CooldownButton.RegisteredButtons[i].IsEffectActive) {
                    CooldownButton.RegisteredButtons[i].OnEffectEnd();
                    CooldownButton.RegisteredButtons[i].IsEffectActive = false;
                }
            }
        }
    }
}
