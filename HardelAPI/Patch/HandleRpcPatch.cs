using HarmonyLib;
using Hazel;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HardelAPI.Utility.Utils;
using HardelAPI.Reactor;
using HardelAPI.Utility.Ability;

namespace HardelAPI.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpcPatch {
        public static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader) {
            if (callId == (byte) CustomRPC.PlaceCamera) {
                Vector2 vector = reader.ReadVector2();
                CameraUtils.AddNewCamera(vector);

                return false;
            }

            if (callId == (byte) CustomRPC.SealVent) {
                int ventId = reader.ReadInt32();
                VentUtils.SealVent(VentUtils.IdToVent(ventId));

                return false;
            }

            if (callId == (byte) CustomRPC.PlaceCameraBuffer) {
                List<Vector2> vectors = reader.ReadListVector2();
                foreach (var vector in vectors)
                    CameraUtils.AddNewCamera(vector);

                return false;
            }

            if (callId == (byte) CustomRPC.SealVentBuffer) {
                List<byte> ventIds = reader.ReadBytesAndSize().ToList();

                HardelApiPlugin.Logger.LogInfo($"ventIds: {ventIds.Count}");
                foreach (var ventId in ventIds) {
                    Vent vent = VentUtils.IdToVent(ventId);
                    HardelApiPlugin.Logger.LogInfo($"ventId: {vent.Id}, ventsToSeal: {vent.name}");
                    VentUtils.SealVent(vent);
                }

                return false;
            }

            if (callId == (byte) CustomRPC.PlaceVent) {
                int id = reader.ReadPackedInt32();
                Vector2 postion = reader.ReadVector2();
                int leftVent = reader.ReadPackedInt32();
                int centerVent = reader.ReadPackedInt32();
                int rightVent = reader.ReadPackedInt32();

                VentUtils.SpawnVent(
                    id: id,
                    postion: postion,
                    leftVent: leftVent,
                    centerVent: centerVent,
                    rightVent: rightVent
                );
                return false;
            }

            if (callId == (byte) CustomRPC.FixLights) {
                SwitchSystem lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                lights.ActualSwitches = lights.ExpectedSwitches;

                return false;
            }

            if (callId == (byte) CustomRPC.Invisibility) {
                byte PlayerId = reader.ReadByte();
                float Duration = reader.ReadSingle();
                List<byte> whiteListIds = reader.ReadBytesAndSize().ToArray().ToList();

                List<PlayerControl> whiteList = PlayerControlUtils.IdListToPlayerControlList(whiteListIds);
                PlayerControl Player = PlayerControlUtils.FromPlayerId(PlayerId);

                Coroutines.Start(Invisbility.Invisibility(Player, Duration, whiteList));
                return false;
            }

            if (callId == (byte) CustomRPC.CleanBody) {
                DeadBody deadBody = DeadBodyUtils.FromParentId(reader.ReadByte());
                float duration = reader.ReadSingle();

                Coroutines.Start(DeadBodyUtils.CleanCoroutine(deadBody, duration));

                return false;
            }

            return true;
        }
    }
}
