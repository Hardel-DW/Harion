using HarmonyLib;
using Hazel;

namespace Harion.Cooldown.Patch {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpcPatch {
        public static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader) {
            if (callId == (byte) CustomRPC.SyncroButton) {
                int buttonId = reader.ReadInt32();
                CooldownButton button = CooldownButton.GetButtonById(buttonId);
                button.ReadData(reader);

                return false;
            }

            return true;
        }
    }
}
