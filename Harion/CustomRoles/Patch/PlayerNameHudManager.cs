using Harion.Utility.Utils;
using HarmonyLib;
using InnerNet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdatePatch {
        public static bool MeetingIsPassed = false;

        public static void Postfix(HudManager __instance) {
            if ((AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started) || (AmongUsClient.Instance.GameMode == GameModes.FreePlay)) {
                if (PlayerControl.LocalPlayer == null || PlayerControl.AllPlayerControls == null || PlayerControl.AllPlayerControls.Count == 0)
                    return;

                foreach (var Role in RoleManager.AllRoles) {
                    if (Role.AllPlayers == null || Role.AllPlayers.Count == 0)
                        continue;

                    if (MeetingHud.Instance != null) 
                        UpdateMeetingHUD(MeetingHud.Instance, Role);

                    foreach (var PlayerHasRole in Role.AllPlayers) {
                        if (!Role.HasRole(PlayerHasRole))
                            continue;

                        string NamePlayer = Role.NameText(PlayerHasRole);

                        if (Role.RoleVisibleByWhitelist.ContainsPlayer(PlayerControl.LocalPlayer))
                            DefineName(PlayerHasRole, Role.Color, NamePlayer);
                    }
                }

                if (RoleManager.GetAllRoles(PlayerControl.LocalPlayer).Count == 0 && PlayerControl.LocalPlayer.Data.IsImpostor) {
                    foreach (PlayerControl Player in PlayerControlUtils.GetImpostors()) {
                        if (RoleManager.GetAllRoles(Player).Count == 0) {
                            if (MeetingHud.Instance != null) 
                                UpdateMeetingVanillaHUD(MeetingHud.Instance, Player);
                            else DefineName(Player, Palette.ImpostorRed, RoleManager.NameTextVanilla(Player));
                        }
                        else {
                            RoleManager role = RoleManager.GetAllRoles(Player).FirstOrDefault();
                            if (MeetingHud.Instance != null) 
                                UpdateMeetingRole(MeetingHud.Instance, Player, role);
                            else DefineName(Player, role.Color, role.NameText(Player));
                        }
                    }
                }

                if (RoleManager.GetAllRoles(PlayerControl.LocalPlayer).Count == 0 && !PlayerControl.LocalPlayer.Data.IsImpostor) {
                    if (MeetingHud.Instance != null)
                        UpdateMeetingVanillaHUD(MeetingHud.Instance, PlayerControl.LocalPlayer);

                    DefineName(PlayerControl.LocalPlayer, Palette.White, RoleManager.NameTextVanilla(PlayerControl.LocalPlayer));
                }

                if (PlayerControl.LocalPlayer.Data.IsDead && MeetingIsPassed && HarionPlugin.DeadSeeAllRoles.GetValue()) {
                    bool CanSee = true;

                    foreach (var Role in RoleManager.GetAllRoles(PlayerControl.LocalPlayer))
                        if (Role.ForceUnshowAllRolesOnMeeting)
                            CanSee = false;

                    if (CanSee) {
                        foreach (var player in PlayerControl.AllPlayerControls) {
                            if (MeetingHud.Instance != null)
                                UpdateMeetingForDead(MeetingHud.Instance, player);

                            if (RoleManager.GetAllRoles(player).Count == 0) {
                                DefineName(player, player.Data.IsImpostor ? Palette.ImpostorRed : Palette.White, RoleManager.NameTextVanilla(player));
                            } else {
                                RoleManager role = RoleManager.GetAllRoles(player).FirstOrDefault();
                                DefineName(player, role.Color, role.NameText(player));
                            }
                        }
                    }
                }

                foreach (var PlayerSpecific in RoleManager.specificNameInformation) {
                    if (MeetingHud.Instance != null)
                        UpdateMeetingForSpecific(MeetingHud.Instance, PlayerSpecific);

                    DefineName(PlayerSpecific.Key, PlayerSpecific.Value.color, RoleManager.NameTextSpecific(PlayerSpecific.Key));
                }

                if (MeetingHud.Instance != null)
                    UpdatePlayerVoteArea(MeetingHud.Instance);
            }

            if (PlayerControl.AllPlayerControls != null)
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    RoleManager.PlayerNamePositon(player);
        }

        // Meeting Management
        public static void UpdateMeetingHUD(MeetingHud __instance, RoleManager Role) {
            foreach (var PlayerHasRole in Role.AllPlayers) {
                foreach (PlayerVoteArea PlayerVA in __instance.playerStates) {
                    PlayerControl Player = PlayerControlUtils.FromPlayerId((byte) PlayerVA.TargetPlayerId);
                    if (PlayerHasRole.PlayerId != Player.PlayerId || !Role.HasRole(PlayerHasRole))
                        continue;

                    string NamePlayer = Role.NameText(PlayerHasRole, PlayerVA);

                    if (Role.RoleVisibleByWhitelist.ContainsPlayer(PlayerControl.LocalPlayer))
                        DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                }
            }
        }

        public static void UpdateMeetingVanillaHUD(MeetingHud __instance, PlayerControl Player) {
            foreach (PlayerVoteArea PlayerVA in __instance.playerStates) {
                if (Player.PlayerId != (byte) PlayerVA.TargetPlayerId)
                    continue;
                
                DefineMeetingName(PlayerVA, Player.Data.IsImpostor ? Palette.ImpostorRed : Palette.White, RoleManager.NameTextVanilla(Player, PlayerVA));
            }
        }

        public static void UpdateMeetingRole(MeetingHud __instance, PlayerControl Player, RoleManager role) {
            foreach (PlayerVoteArea PlayerVA in __instance.playerStates) {
                if (Player.PlayerId != (byte) PlayerVA.TargetPlayerId)
                    continue;

                DefineMeetingName(PlayerVA, role.Color, role.NameText(Player, PlayerVA));
            }
        }

        public static void UpdateMeetingForSpecific(MeetingHud __instance, KeyValuePair<PlayerControl, (Color color, string name)> playerSpecific) {
            foreach (PlayerVoteArea PlayerVA in __instance.playerStates) {
                if (playerSpecific.Key.PlayerId != (byte) PlayerVA.TargetPlayerId)
                    continue;

                DefineMeetingName(PlayerVA, playerSpecific.Value.color, RoleManager.NameTextSpecific(playerSpecific.Key, PlayerVA));
            }
        }

        public static void UpdateMeetingForDead(MeetingHud __instance, PlayerControl Player) {
            foreach (PlayerVoteArea PlayerVA in __instance.playerStates) {
                if (Player.PlayerId != (byte) PlayerVA.TargetPlayerId)
                    continue;

                if (RoleManager.GetAllRoles(Player).Count == 0) {
                    DefineMeetingName(PlayerVA, Player.Data.IsImpostor ? Palette.ImpostorRed : Palette.White, RoleManager.NameTextVanilla(Player, PlayerVA));
                } else {
                    RoleManager role = RoleManager.GetAllRoles(Player).FirstOrDefault();
                    DefineMeetingName(PlayerVA, role.Color, role.NameText(Player, PlayerVA));
                }
            }
        }

        // Set display Name
        public static void DefineName(PlayerControl Player, Color color, string newName) {
            Player.nameText.text = newName;
            Player.nameText.color = color;
        }

        public static void DefineMeetingName(PlayerVoteArea Player, Color color, string newName) {
            Player.NameText.text = newName;
            Player.NameText.color = color;
        }

        // Position MeetingHud
        public static void UpdatePlayerVoteArea(MeetingHud Instance) {
            foreach (PlayerVoteArea Area in Instance.playerStates) {
                Area.NameText.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Left;
                Vector3 Position = Area.NameText.transform.localPosition;
                Position.ChangeX(0.375f);

                if (!Area.NameText.text.Contains("\n"))
                    Position.ChangeY(0.125f);

                Area.NameText.transform.localPosition = Position;
            }
        }
    }
}
