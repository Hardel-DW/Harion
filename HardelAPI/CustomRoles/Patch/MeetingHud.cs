using HarmonyLib;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Awake))]
    public static class MeetingStartPatch {
        public static void Postfix(MeetingHud __instance) {
            Plugin.Logger.LogInfo($"{PlayerControl.LocalPlayer.Data.IsDead}, {HudUpdatePatch.MeetingIsPassed}");
            if (PlayerControl.LocalPlayer.Data.IsDead && !HudUpdatePatch.MeetingIsPassed)
                HudUpdatePatch.MeetingIsPassed = true;

            foreach (var Role in RoleManager.AllRoles)
                Role.OnMeetingStart(__instance);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingUpdatePatch {
        public static void Postfix(MeetingHud __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnMeetingUpdate(__instance);
        }
    }


    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class MeetingClosePatch {
        public static void Postfix(MeetingHud __instance) {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnMeetingEnd(__instance);
        }
    }
}
