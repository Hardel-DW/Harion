using HardelAPI.Utility;
using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;

namespace HardelAPI.CustomRoles.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpcPatch {
        public static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader) {
            if (callId == 250) {
                RoleManager Role = RoleManager.GerRoleById(reader.ReadByte());
                List<byte> selectedPlayers = reader.ReadBytesAndSize().ToList();

                Role.ClearRole();
                for (int i = 0; i < selectedPlayers.Count; i++)
                    Role.AddPlayer(PlayerControlUtils.FromPlayerId(selectedPlayers[i]));

                return false;
            }

            if (callId == (byte) CustomRPC.ForceEndGame) {
                RoleManager Role = RoleManager.GerRoleById(reader.ReadByte());
                List<byte> selectedPlayers = reader.ReadBytesAndSize().ToList();
                List<PlayerControl> WinPlayer = PlayerControlUtils.IdListToPlayerControlList(selectedPlayers);

                var playerLoses = PlayerControl.AllPlayerControls;
                foreach (var playerLose in playerLoses.ToArray().ToList())
                    foreach (var Player in WinPlayer)
                        if (playerLose.PlayerId == Player.PlayerId)
                            playerLoses.Remove(playerLose);

                // Set PlayerWin
                foreach (var player in WinPlayer) {
                    player.Revive();
                    player.Data.IsDead = false;
                    player.Data.IsImpostor = true;
                }

                // Set PlayerLose
                foreach (var player in PlayerControl.AllPlayerControls) {
                    player.RemoveInfected();
                    player.Die(DeathReason.Exile);
                    player.Data.IsDead = true;
                    player.Data.IsImpostor = false;
                }

                Role.HasWin = true;
                return false;
            }

            return true;
        }
    }
}
