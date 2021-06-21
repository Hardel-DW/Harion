/*using Harion.Utility.Utils;
using HarmonyLib;
using Hazel;

namespace Harion.Crowded {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpcPatch {
        public static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader) {
            if (callId == (byte) CustomRPC.VotingComplete) {
                byte[] states = reader.ReadBytesAndSize();
                byte[] votes = reader.ReadBytesAndSize();
                byte exiled = reader.ReadByte();

                if (MeetingHud.Instance != null)
                    MeetingHud.Instance.CustomVotingComplete(states, votes, GameData.Instance.GetPlayerById(exiled), exiled == byte.MaxValue);

                return false;
            }

            if (callId == (byte) CustomRPC.SetColor) {
                byte playerId = reader.ReadByte();
                byte data = reader.ReadByte();

                PlayerControlUtils.FromNetId(playerId).SetColor(data);

                return false;
            }

            return true;
        }

        internal static void SendVotingComplete(byte[] states, byte[] votes, byte exiled) {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.PlaceCameraBuffer, SendOption.Reliable, -1);
            messageWriter.WriteBytesAndSize(states);
            messageWriter.WriteBytesAndSize(votes);
            messageWriter.Write(exiled);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

            if (MeetingHud.Instance != null)
                MeetingHud.Instance.CustomVotingComplete(states, votes, GameData.Instance.GetPlayerById(exiled), exiled == byte.MaxValue);
        }

        internal static void SendSetColor(byte playerId, byte data) {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.PlaceCameraBuffer, SendOption.Reliable, -1);
            messageWriter.Write(playerId);
            messageWriter.Write(data);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

            PlayerControlUtils.FromNetId(playerId).SetColor(data);
        }
    }
}
*/