using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace HardelAPI.Utility.CustomRoles.Patch {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillHudManager {
        private static KillButtonManager KillButton;

        public static void Postfix(HudManager __instance) {
            KillButton = __instance.KillButton;
            
            foreach (var Role in RoleManager.AllRoles) {
                if (PlayerControl.LocalPlayer == null || PlayerControl.AllPlayerControls == null)
                    break;

                if (PlayerControl.LocalPlayer.Data == null || !(PlayerControl.AllPlayerControls.Count > 1))
                    break;

                if (Role.WhiteListKill == null)
                    if (Role.CanKill != Enumerations.PlayerSide.Nobody || PlayerControl.LocalPlayer.Data.IsImpostor)
                        Role.DefineKillWhiteList();

                if (Role.KillCooldown == 0f && PlayerControl.LocalPlayer.Data.IsImpostor)
                    Role.KillCooldown = PlayerControl.GameOptions.KillCooldown;

                if ((AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started) || (AmongUsClient.Instance.GameMode == GameModes.FreePlay)) {
                    if (Role.HasRole(PlayerControl.LocalPlayer) && Role.WhiteListKill != null) {
                        PlayerControl ClosestPlayer = Role.GetClosestTarget(PlayerControl.LocalPlayer);
                        
                        if (PlayerControl.LocalPlayer.Data.IsDead) {
                            KillButton.gameObject.SetActive(false);
                            KillButton.isActive = false;
                        } else {
                            KillButton.gameObject.SetActive(!MeetingHud.Instance);
                            KillButton.isActive = !MeetingHud.Instance;
                            KillButton.SetCoolDown(Role.KillTimer(), Role.KillCooldown);

                            if (Input.GetKeyDown(KeyCode.Q))
                                KillButton.PerformKill();
                            
                            float distBetweenPlayers = Vector3.Distance(PlayerControl.LocalPlayer.transform.position, ClosestPlayer.transform.position);

                            if ((distBetweenPlayers < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance]) && KillButton.enabled)
                                KillButton.SetTarget(ClosestPlayer);
                        }

                        break;
                    } else if (PlayerControl.LocalPlayer.Data.IsImpostor && !PlayerControl.LocalPlayer.Data.IsDead) {
                        __instance.KillButton.gameObject.SetActive(!MeetingHud.Instance);
                        __instance.KillButton.isActive = !MeetingHud.Instance;

                        break;
                    } else {
                        KillButton.gameObject.SetActive(false);
                        KillButton.isActive = false;
                    }
                }
            }
        }
    }
}
