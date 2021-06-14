using HardelAPI.Utility.Helper;
using HarmonyLib;
using Hazel;
using System.Linq;

namespace HardelAPI.CustomOptions {

    [HarmonyPatch]
    public partial class CustomOption {

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader) {
            if (callId == (byte) CustomRPC.SyncroCustomGameOption) {
                byte[] sha1 = reader.ReadBytes(SHA1Helper.Length);
                CustomOptionType type = (CustomOptionType) reader.ReadByte();
                CustomOption customOption = Options.FirstOrDefault(option => option.Type == type && option.SHA1.SequenceEqual(sha1));

                if (customOption == null) {
                    HardelApiPlugin.Logger.LogWarning($"Received option that could not be found, sha1: \"{string.Join("", sha1.Select(b => $"{b:X2}"))}\", type: {type}.");
                    return false;
                }

                object value = null;
                if (type == CustomOptionType.Toggle)
                    value = reader.ReadBoolean();
                else if (type == CustomOptionType.Number)
                    value = reader.ReadSingle();
                else if (type == CustomOptionType.String)
                    value = reader.ReadInt32();

                if (Debug)
                    HardelApiPlugin.Logger.LogInfo($"\"{customOption.ID}\" type: {type}, value: {value}, current value: {customOption.Value}");

                customOption.SetValue(value, true);

                if (Debug)
                    HardelApiPlugin.Logger.LogInfo($"\"{customOption.ID}\", set value: {customOption.Value}");

                return false;
            }

            return true;
        }

        public static void SendSyncro(CustomOption option) {
            byte[] sha1 = option.SHA1;
            CustomOptionType type = option.Type;
            object value = option.GetValue<object>();

            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SyncroCustomGameOption, SendOption.Reliable, -1);
            messageWriter.Write(sha1);
            messageWriter.Write((byte) type);

            if (type == CustomOptionType.Toggle)
                messageWriter.Write((bool) value);
            else if (type == CustomOptionType.Number)
                messageWriter.Write((float) value);
            else if (type == CustomOptionType.String)
                messageWriter.Write((int) value);

            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }
    }
}
