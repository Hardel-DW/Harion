using HardelAPI.Data;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardelAPI.Utility {
    public static class VentUtils {

		public static void RpcSealMultipleVent(List<byte> ventIds) {
            foreach (var ventId in ventIds) {
				Vent vent = IdToVent(ventId);
				SealVent(vent);
            }

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SealVentBuffer, SendOption.Reliable, -1);
			messageWriter.WriteBytesAndSize(ventIds.ToArray());
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void RpcSealMultipleVent(List<Vent> vents) {
			Plugin.Logger.LogInfo($"ventIds: {vents.Count}");
			foreach (var vent in vents) {
				Plugin.Logger.LogInfo($"ventId: {vent.Id}, ventsToSeal: {vent.name}");

				SealVent(vent);
			}

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SealVentBuffer, SendOption.Reliable, -1);
			messageWriter.WriteBytesAndSize(VentsToList(vents).ToArray());
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void RpcSealVent(Vent vent) {
			SealVent(vent);

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SealVent, SendOption.Reliable, -1);
			messageWriter.Write(vent.Id);
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void SealVent(Vent vent) {
			if (vent == null)
				return;

			Plugin.Logger.LogInfo($"Seal vent:  {vent.Id}");

			Sprite staticSealdedVent = HelperSprite.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.StaticVentSealed.png", 150f);
			Sprite animatedSealeddVent = HelperSprite.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.AnimatedVentSealed.png", 150f);

			PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
			animator?.Stop();
			vent.myRend.sprite = animator == null ? staticSealdedVent : animatedSealeddVent;
			vent.name = "SealedVent_" + vent.name;

			new DangerPoint(vent.transform.position, Palette.CrewmateBlue, vent.name);
		}

		public static void UnsealVents() {
			if (ShipStatus.Instance == null)
				return;

			foreach (Vent vent in ShipStatus.Instance.AllVents) {
				if (vent == null)
					continue;

				if (vent.name.StartsWith("SealedVent_")) {
					PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
					animator?.Play();
					vent.myRend.sprite = GetVentSprite();
					vent.name = "Vent_" + vent.name;
				}
			}
		}

		public static Vent IdToVent(int id) {
			return Object.FindObjectsOfType<Vent>().FirstOrDefault(v => v.Id == id);
		}

		public static List<byte> VentsToList(List<Vent> vents) {
			List<byte> buffer = new List<byte>();
            foreach (var vent in vents)
				buffer.Add((byte) vent.Id);

			return buffer;
		}

		public static Vent GetClosestVent(PlayerControl PlayerReference) {
			double closestDistance = double.MaxValue;
			Vent result = null;

			for (int i = 0; i < ShipStatus.Instance.AllVents.Length; i++) {
				Vent vent = ShipStatus.Instance.AllVents[i];
				if (vent.gameObject.name.StartsWith("SealedVent_"))
					continue;

				float distance = Vector2.Distance(vent.transform.position, PlayerReference.GetTruePosition());
				if (distance <= vent.UsableDistance && distance < closestDistance) {
					closestDistance = distance;
					result = vent;
				}
			}

			return result;
		}

		public static Sprite GetVentSprite() {
			Vent vent;
			if (ShipStatus.Instance == null)
				return null;

			vent = ShipStatus.Instance.AllVents.FirstOrDefault(v => !v.name.StartsWith("SealedVent_"));
			if (vent == null)
				return null;

			return vent?.GetComponent<SpriteRenderer>()?.sprite;
		}
	}
}
