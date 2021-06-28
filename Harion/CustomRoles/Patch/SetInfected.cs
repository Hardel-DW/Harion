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

        private static Random random = new Random();

        public static void Prefix() {
            foreach (var Role in RoleManager.AllRoles)
                Role.OnInfectedStart();
        }

        public static void Postfix() {
            if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
                return;

            List<PlayerControl> playersList = PlayerControl.AllPlayerControls.ToArray().ToList();
            RoleManager.ClearAllRoles();
            List<RoleManager> Roles = RoleManager.AllRoles.OrderBy(e => random.Next()).ToList();

            foreach (RoleManager Role in Roles) {
                if (!(Role.Side == PlayerSide.Everyone || Role.Side == PlayerSide.Crewmate || Role.Side == PlayerSide.Impostor))
                    throw new Exception($"Error in the selection of players, for the {Role.Name} Role. \n The player Side has only three possible values: Crewmate, Impostors or Everyone, Given: {Role.Side}");

                int PercentApparition = new Random().Next(0, 100);

                if (playersList != null && playersList.Count > 0 && Role.RoleActive && Role.NumberPlayers > 0 && Role.PercentApparition > PercentApparition) {
                    int crewmateRemaining = PlayerControl.AllPlayerControls.ToArray().ToList().Count(p => !p.Data.IsImpostor);
                    int impostorRemaining = PlayerControl.AllPlayerControls.ToArray().ToList().Count(p => p.Data.IsImpostor);
                    if (Role.Side == PlayerSide.Crewmate && crewmateRemaining < 1)
                        continue;

                    if (Role.Side == PlayerSide.Impostor && impostorRemaining < 1)
                        continue;

                    if (Role.OnRoleSelectedInInfected(playersList))
                        continue;

                    MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SetRole, SendOption.None, -1);
                    messageWriter.Write(Role.RoleId);
                    List<byte> playerSelected = new List<byte>();

                    for (int i = 0; i < Role.NumberPlayers; i++) {
                        List<PlayerControl> PlayerSelectable = playersList.ToArray().ToList();
                        if (Role.Side == PlayerSide.Impostor)
                            PlayerSelectable.RemoveAll(x => !x.Data.IsImpostor);

                        if (Role.Side == PlayerSide.Crewmate)
                            PlayerSelectable.RemoveAll(x => x.Data.IsImpostor);

                        if (PlayerSelectable != null && PlayerSelectable.Count > 0) {
                            PlayerControl selectedPlayer = PlayerSelectable[random.Next(0, PlayerSelectable.Count)];
                            Role.AllPlayers.AddPlayer(selectedPlayer);
                            playersList.Remove(selectedPlayer);
                            playerSelected.Add(selectedPlayer.PlayerId);
                        }
                    }

                    messageWriter.WriteBytesAndSize(playerSelected.ToArray());
                    AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
    public static class LocalSetInfectedPatch {
        public static void Postfix(PlayerControl __instance) {
            foreach (var Role in RoleManager.AllRoles) {
                Role.OnInfectedEnd();
                Role.DefineVisibleByWhitelist();
            }
        }
    }
}
