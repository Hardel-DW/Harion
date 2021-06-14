using HarmonyLib;
using HardelAPI.Enumerations;
using System.Collections.Generic;
using System.Collections;
using HardelAPI.Reactor;
using UnityEngine;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    public static class TasksPatch {
        public static void Postfix(PlayerControl __instance) {
            if (PlayerControl.LocalPlayer == null)
                return;

            if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                return;

            Coroutines.Start(SetRoleTask());
        }

        private static IEnumerator SetRoleTask() {
            while (!ShipStatus.Instance) {
                yield return null;
            }

            yield return new WaitForSeconds(1);

            List<RoleManager> Roles = RoleManager.GetAllRoles(PlayerControl.LocalPlayer);
            foreach (RoleManager Role in Roles)
                if (Role.GiveTasksAt == Moment.StartGame)
                    Role.AddImportantTasks(PlayerControl.LocalPlayer);

            yield break;
        }
    }
}
