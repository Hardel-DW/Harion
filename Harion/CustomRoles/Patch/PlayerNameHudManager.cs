using Harion.ColorDesigner;
using Harion.Utility.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.CustomRoles.Patch {

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdatePatch {
        public static bool MeetingIsPassed = false;
        public static bool CanSeeAfterMeeting => MeetingIsPassed && GenericGameOptions.DeadSeeAllRoles.GetValue() && PlayerControl.LocalPlayer.Data.IsDead;

        public static void Postfix(HudManager __instance) {
            UpdatePlayerVoteArea(MeetingHud.Instance);
            if (!GameUtils.GameStarted || PlayerControlUtils.IsPlayerNull)
                return;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                PlayerVoteArea PlayerVoteArea = PlayerVoteAreaFromPlayerControl(player);
                RoleManager role = RoleManager.GetRoleToDisplay(player);

                if (role == null) {
                    if ((player.Data.IsImpostor && PlayerControl.LocalPlayer.Data.IsImpostor) || (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) || CanSeeAfterMeeting) {
                        (string name, Color? color) PlayerInfo = RoleManager.NameTextDefault(player);
                        DefineName(player, PlayerVoteArea, PlayerInfo.name, PlayerInfo.color);
                    }
                } else if (role.RoleVisibleByWhitelist.ContainsPlayer(PlayerControl.LocalPlayer) || CanSeeAfterMeeting)
                    DefineName(player, PlayerVoteArea, role.NameText(player), role.Color);

                if (!GenericGameOptions.ShowAllSecondaryRole.GetValue() || PlayerVoteArea != null) {
                    SpecificData SpecificData = RoleManager.GetSpecificData(player).FirstOrDefault();
                    if (SpecificData != null)
                        DefineName(player, PlayerVoteArea, SpecificData.Name, SpecificData.Color);
                } else
                    AllInformations(player, role);
            }
        }

        private static void DefineName(PlayerControl Player, PlayerVoteArea PlayerVA, string name, Color? color = null) {
            Player.nameText.text = name;
            if (color != null)
                Player.nameText.color = color.Value;

            if (PlayerVA != null && MeetingHud.Instance != null) {
                PlayerVA.NameText.text = name;
                if (color != null) {
                    PlayerVA.NameText.color = color.Value;
                }
            }
        }

        private static void UpdatePlayerVoteArea(MeetingHud Instance) {
            if (Instance == null)
                return;

            foreach (PlayerVoteArea Area in Instance.playerStates) {
                Area.NameText.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Left;
                Vector3 Position = Area.NameText.transform.localPosition;
                Position.ChangeX(0.375f);

                Area.NameText.transform.localPosition = Position;
            }
        }

        private static PlayerVoteArea PlayerVoteAreaFromPlayerControl(PlayerControl Player) {
            if (MeetingHud.Instance == null)
                return null;

            return MeetingHud.Instance.playerStates.FirstOrDefault(playerVA => playerVA.TargetPlayerId == Player.PlayerId);
        }

        private static void AllInformations(PlayerControl Player, RoleManager ExcludeRole) {
            if (Player == null)
                return;

            string result = Player.name;
            List<RoleManager> AllRoles = RoleManager.GetAllRoles(Player);
            List<SpecificData> SpecificDatas = RoleManager.GetSpecificData(Player);
            if (ExcludeRole != null && AllRoles != null && AllRoles.Count > 0) {
                AllRoles.RemoveAll(role => role.RoleId == ExcludeRole.RoleId);
                AppendName(ExcludeRole.Name, ExcludeRole.Color);
            }

            AllRoles.ForEach(element => AppendName(element.Name, element.Color));

            if (SpecificDatas != null && SpecificDatas.Count > 0)
                SpecificDatas.ForEach(element => AppendName(element.Name, element.Color));

            Player.nameText.text = result;
            void AppendName(string name, Color color) => result += $"\n<color={ColorCreator.ColorToHexaString(color)}>{name}</color>";
        }
    }
}