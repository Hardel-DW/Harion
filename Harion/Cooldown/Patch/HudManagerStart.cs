using HarmonyLib;

namespace Harion.Cooldown.Patch {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    class HudManagerStart {
        public static void Postfix(HudManager __instance) {
            foreach (CooldownButton cooldownButton in CooldownButton.RegisteredButtons)
                cooldownButton.CreateButton(__instance);
        }
    }
}
