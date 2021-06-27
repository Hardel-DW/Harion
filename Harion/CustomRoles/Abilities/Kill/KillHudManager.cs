using Harion.CustomKeyBinds.Patch;
using Harion.Enumerations;
using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace Harion.CustomRoles.Abilities.Kill {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillHudManager {
        private static KillButtonManager KillButton;

        public static void Postfix(HudManager __instance) {
            KillButton = __instance.KillButton;
            if (PlayerControl.LocalPlayer == null || PlayerControl.AllPlayerControls == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null || PlayerControl.AllPlayerControls.Count < 1)
                return;

            RoleManager Role = RoleManager.GetMainRole(PlayerControl.LocalPlayer);
            KillAbility KillAbility = Role?.GetAbility<KillAbility>();
            if (KillAbility == null)
                return;

            if (KillAbility.WhiteListKill == null)
                if (KillAbility.CanKill != Killable.Nobody || PlayerControl.LocalPlayer.Data.IsImpostor)
                    KillAbility.DefineKillWhiteList();

            if (KillAbility.KillCooldown == 0f && PlayerControl.LocalPlayer.Data.IsImpostor)
                KillAbility.KillCooldown = PlayerControl.GameOptions.KillCooldown;

            if ((AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started) || (AmongUsClient.Instance.GameMode == GameModes.FreePlay)) {
                if (KillAbility.WhiteListKill != null) {
                    PlayerControl ClosestPlayer = KillAbility.GetClosestTarget(PlayerControl.LocalPlayer);
                        
                    if (PlayerControl.LocalPlayer.Data.IsDead) {
                        KillButton.gameObject.SetActive(false);
                        KillButton.isActive = false;
                    } 
                    else {
                        KillButton.gameObject.SetActive(!MeetingHud.Instance);
                        KillButton.isActive = !MeetingHud.Instance;
                        KillButton.SetCoolDown(KillAbility.KillTimer(), KillAbility.KillCooldown);

                        if (Input.GetKeyDown(KeyBindPatch.Kill.Key))
                            KillButton.PerformKill();
                            
                        float distBetweenPlayers = Vector3.Distance(PlayerControl.LocalPlayer.transform.position, ClosestPlayer.transform.position);
                        if ((distBetweenPlayers < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance]) && KillButton.enabled)
                            KillButton.SetTarget(ClosestPlayer);
                    }

                } else if (PlayerControl.LocalPlayer.Data.IsImpostor && !PlayerControl.LocalPlayer.Data.IsDead) {
                    __instance.KillButton.gameObject.SetActive(!MeetingHud.Instance);
                    __instance.KillButton.isActive = !MeetingHud.Instance;
                } 
                else {
                    KillButton.gameObject.SetActive(false);
                    KillButton.isActive = false;
                }
            }
        }
    }
}
