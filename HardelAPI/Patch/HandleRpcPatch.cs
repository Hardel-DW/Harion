using HarmonyLib;
using Hazel;
using UnityEngine;
using HardelAPI.Utility;
using System.Collections.Generic;
using System.Linq;

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

                Plugin.Logger.LogInfo($"ventIds: {ventIds.Count}");
                foreach (var ventId in ventIds) {
                    Vent vent = VentUtils.IdToVent(ventId);
                    Plugin.Logger.LogInfo($"ventId: {vent.Id}, ventsToSeal: {vent.name}");
                    VentUtils.SealVent(vent);
                }

                return false;
            }

            return true;
        }
    }
}
