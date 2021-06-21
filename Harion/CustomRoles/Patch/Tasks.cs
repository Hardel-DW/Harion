using HarmonyLib;
using Harion.Enumerations;
using System.Collections.Generic;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__83), nameof(PlayerControl._CoSetTasks_d__83.MoveNext))]
    public static class PlayerControlSetTashPatch {

        public static void Postfix(PlayerControl._CoSetTasks_d__83 __instance) {
            if (PlayerControl.LocalPlayer == null)
                return;

            PlayerControl Player = __instance.__4__this;
            if (Player.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                return;

            List<RoleManager> Roles = RoleManager.GetAllRoles(PlayerControl.LocalPlayer);
            foreach (RoleManager Role in Roles)
                if (Role.GiveTasksAt == Moment.StartGame)
                    Role.AddImportantTasks(PlayerControl.LocalPlayer);
        }
    }
}
