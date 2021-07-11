using HarmonyLib;
using Hazel;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Harion.Utility.Utils;
using Harion.Reactor;
using Harion.Utility.Ability;

namespace Harion.Patch {

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

                foreach (var ventId in ventIds) {
                    Vent vent = VentUtils.IdToVent(ventId);
                    VentUtils.SealVent(vent);
                }

                return false;
            }

            if (callId == (byte) CustomRPC.PlaceVent) {
                int id = reader.ReadInt32();
                Vector3 postion = reader.ReadVector3();
                int leftVent = reader.ReadInt32();
                int centerVent = reader.ReadInt32();
                int rightVent = reader.ReadInt32();

                VentUtils.PlaceLocalVent(id, postion, leftVent, centerVent, rightVent);
                return false;
            }

            if (callId == (byte) CustomRPC.FixLights) {
                SaboatageUtils.FixLight();

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
