using HarmonyLib;
using Hazel;
using Harion.Enumerations;
using Harion.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Harion.CustomRoles.Patch {

    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public static class SetInfectedPatch {
        public static void Prefix() {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnInfectedStart();
        }

        public static void Postfix() {
            List<PlayerControl> playersList = PlayerControl.AllPlayerControls.ToArray().ToList();
            RoleManager.ClearAllRoles();

            foreach (var Role in RoleManager.AllRoles) {
                if (!(Role.Side == PlayerSide.Everyone || Role.Side == PlayerSide.Crewmate || Role.Side == PlayerSide.Impostor))
                    throw new Exception($"Error in the selection of players, for the {Role.Name} Role. \n The player Side has only three possible values: Crewmate, Impostors or Everyone, Given: {Role.Side}");

                int PercentApparition = new Random().Next(0, 100);

                HarionPlugin.Logger.LogInfo($"Role: {Role.Name}, Active: {Role.RoleActive}, PercentApparition: {Role.PercentApparition}, Number Player: {Role.NumberPlayers}, Percent Apparition: {PercentApparition}");
                if (playersList != null && playersList.Count > 0 && Role.RoleActive && Role.PercentApparition > PercentApparition) {
                    bool condition = Role.OnRoleSelectedInInfected(playersList);
                    if (condition)
                        continue;

                    MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SetRole, SendOption.None, -1);
                    messageWriter.Write(Role.RoleId);
                    List<byte> playerSelected = new List<byte>();

                    for (int i = 0; i < Role.NumberPlayers; i++) {
                        List<PlayerControl> PlayerSelectable = playersList;
                        if (Role.Side == PlayerSide.Impostor)
                            PlayerSelectable.RemoveAll(x => !x.Data.IsImpostor);

                        if (Role.Side == PlayerSide.Crewmate)
                            PlayerSelectable.RemoveAll(x => x.Data.IsImpostor);

                        if (PlayerSelectable != null && PlayerSelectable.Count > 0) {
                            Random random = new Random();
                            PlayerControl selectedPlayer = PlayerSelectable[random.Next(0, PlayerSelectable.Count)];
                            Role.AllPlayers.AddPlayer(selectedPlayer);
                            playersList.Remove(selectedPlayer);
                            playerSelected.Add(selectedPlayer.PlayerId);
                            HarionPlugin.Logger.LogInfo($"Role: {Role.Name}, Given to: {selectedPlayer.nameText.text}");
                        }
                    }

                    messageWriter.WriteBytesAndSize(playerSelected.ToArray());
                    AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                }
            }

            if (AmongUsClient.Instance.AmHost) {
                foreach (var Role in RoleManager.AllRoles) {
                    Role.OnInfectedEnd();
                    Role.DefineVisibleByWhitelist();
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
    public static class LocalSetInfectedPatch {
        public static void Postfix(PlayerControl __instance) {
            if (!AmongUsClient.Instance.AmHost) {
                foreach (var Role in RoleManager.AllRoles) {
                    Role.OnInfectedEnd();
                    Role.DefineVisibleByWhitelist();
                }
            }
        }
    }
}
