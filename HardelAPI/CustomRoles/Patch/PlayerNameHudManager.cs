using HardelAPI.Utility;
using HarmonyLib;
using InnerNet;
using System.Linq;
using UnityEngine;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdatePatch {
        public static bool MeetingIsPassed = false;
        private static Vector3 oldScale = Vector3.zero;
        private static Vector3 oldPosition = Vector3.zero;

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
                        switch (Role.VisibleBy) {
                            case Enumerations.PlayerSide.Self:
                                if (PlayerHasRole.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                            case Enumerations.PlayerSide.Impostor:
                                if (PlayerControl.LocalPlayer.Data.IsImpostor)
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                            case Enumerations.PlayerSide.Crewmate:
                                if (!PlayerControl.LocalPlayer.Data.IsImpostor)
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                            case Enumerations.PlayerSide.Everyone:
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                            case Enumerations.PlayerSide.Dead:
                                if (PlayerControl.LocalPlayer.Data.IsDead)
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                            case Enumerations.PlayerSide.DeadCrewmate:
                                if (PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsImpostor)
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                            case Enumerations.PlayerSide.DeadImpostor:
                                if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Data.IsImpostor)
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                            case Enumerations.PlayerSide.SameRole:
                                if (Role.HasRole(PlayerControl.LocalPlayer.PlayerId))
                                    DefineName(PlayerHasRole, Role.Color, NamePlayer);
                            break;
                        }
                    }
                }

                if (RoleManager.GetAllRoles(PlayerControl.LocalPlayer).Count == 0 && PlayerControl.LocalPlayer.Data.IsImpostor) {
                    foreach (var Player in PlayerControl.AllPlayerControls.ToArray().ToList().Where(s => s.Data.IsImpostor)) {
                        if (RoleManager.GetAllRoles(Player).Count == 0 && PlayerControl.LocalPlayer.Data.IsImpostor) {
                            if (MeetingHud.Instance != null) UpdateMeetingVanillaHUD(MeetingHud.Instance, Player);
                            else DefineName(Player, Palette.ImpostorRed, RoleManager.NameTextVanilla(Player));
                        }
                        else {
                            RoleManager role = RoleManager.GetAllRoles(Player).FirstOrDefault();
                            if (MeetingHud.Instance != null) UpdateMeetingRole(MeetingHud.Instance, Player, role);
                            else DefineName(Player, role.Color, role.NameText(Player));
                        }
                    }
                }

                if (RoleManager.GetAllRoles(PlayerControl.LocalPlayer).Count == 0 && !PlayerControl.LocalPlayer.Data.IsImpostor) {
                    if (MeetingHud.Instance != null)
                        UpdateMeetingVanillaHUD(MeetingHud.Instance, PlayerControl.LocalPlayer);

                    DefineName(PlayerControl.LocalPlayer, Palette.White, RoleManager.NameTextVanilla(PlayerControl.LocalPlayer));
                }

                if (PlayerControl.LocalPlayer.Data.IsDead && MeetingIsPassed && Plugin.DeadSeeAllRoles.GetValue()) {
                    bool CanSee = true;

                    foreach (var Role in RoleManager.GetAllRoles(PlayerControl.LocalPlayer))
                        if (Role.ForceUnshowAllRolesOnMeeting)
                            CanSee = false;

                    if (CanSee) {
                        foreach (var player in PlayerControl.AllPlayerControls) {
                            if (MeetingHud.Instance != null)
                                UpdateMeetingForDead(MeetingHud.Instance, player);

                            if (RoleManager.GetAllRoles(player).Count == 0) {
                                DefineName(player, player.Data.IsImpostor ? Palette.ImpostorRed : Palette.White, RoleManager.NameTextVanilla(PlayerControl.LocalPlayer));
                            } else {
                                RoleManager role = RoleManager.GetAllRoles(player).FirstOrDefault();
                                DefineName(player, role.Color, role.NameText(player));
                            }
                        }
                    }
                }
            }
        }

        // Meeting Management
        public static void UpdateMeetingHUD(MeetingHud __instance, RoleManager Role) {
            foreach (var PlayerHasRole in Role.AllPlayers) {
                foreach (PlayerVoteArea PlayerVA in __instance.playerStates) {
                    PlayerControl Player = PlayerControlUtils.FromPlayerId((byte) PlayerVA.TargetPlayerId);
                    if (PlayerHasRole.PlayerId != Player.PlayerId || !Role.HasRole(PlayerHasRole))
                        continue;

                    string NamePlayer = Role.NameText(PlayerHasRole, PlayerVA);
                    switch (Role.VisibleBy) {
                        case Enumerations.PlayerSide.Self:
                        if (PlayerControl.LocalPlayer.PlayerId == PlayerHasRole.PlayerId)
                            DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                        case Enumerations.PlayerSide.Impostor:
                        if (PlayerControl.LocalPlayer.Data.IsImpostor)
                            DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                        case Enumerations.PlayerSide.Crewmate:
                        if (!PlayerControl.LocalPlayer.Data.IsImpostor)
                            DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                        case Enumerations.PlayerSide.Everyone:
                        DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                        case Enumerations.PlayerSide.Dead:
                        if (PlayerControl.LocalPlayer.Data.IsDead)
                            DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                        case Enumerations.PlayerSide.DeadCrewmate:
                        if (PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsImpostor)
                            DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                        case Enumerations.PlayerSide.DeadImpostor:
                        if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Data.IsImpostor)
                            DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                        case Enumerations.PlayerSide.SameRole:
                        if (Role.HasRole(PlayerControl.LocalPlayer.PlayerId))
                            DefineMeetingName(PlayerVA, Role.Color, NamePlayer);
                        break;
                    }
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

            if (Player.NameText.text.Contains("\n")) {
                // Store Old Scale
                Vector3 vector = Vector3.one * 1.8f;
                Vector3 localScale = Player.NameText.transform.localScale;
                if (vector != localScale)
                    oldScale = localScale;

                // Store Old Position
                Vector3 vector2 = new Vector3(1.43f, 0.055f, 0f);
                Vector3 localPosition = Player.NameText.transform.localPosition;
                if (vector2 != localPosition)
                    oldPosition = localPosition;

                // Define Postion and Scale
                Player.NameText.transform.localPosition = vector2;
                Player.NameText.transform.localScale = vector;
            } else {
                // REDefine to old Position
                if (oldPosition != Vector3.zero)
                    Player.NameText.transform.localPosition = oldPosition;
                if (oldScale != Vector3.zero)
                    Player.NameText.transform.localScale = oldScale;
            }
        }
    }
}
