using HarmonyLib;

namespace Harion.CustomRoles.FreeplayTaskTester {

    [HarmonyPatch(typeof(TaskAddButton), nameof(TaskAddButton.Start))]
    public static class TaskAddButtonStartPatch {
        public static bool Prefix(TaskAddButton __instance) {
            return !__instance.name.Contains("FileRolesHarion");
        }
    }
}