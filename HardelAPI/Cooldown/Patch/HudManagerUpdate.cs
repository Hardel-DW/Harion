using HarmonyLib;

namespace HardelAPI.Cooldown.Patch {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdatePatch {
        public static void Postfix() {
            CooldownButton.HudUpdate();
        }
    }
}
