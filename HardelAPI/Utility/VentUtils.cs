using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardelAPI.Utility {
    public static class VentUtils {

		public static void RpcSealMultipleVent(List<byte> ventIds) {
			if (AmongUsClient.Instance.AmHost)
                foreach (var ventId in ventIds)
					SealVent(IdToVent(ventId));

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SealVent, SendOption.Reliable, -1);
			messageWriter.WriteBytesAndSize(ventIds.ToArray());
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void RpcSealMultipleVent(List<Vent> vents) {
			if (AmongUsClient.Instance.AmHost)
				foreach (var vent in vents)
					SealVent(vent);

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SealVent, SendOption.Reliable, -1);
			messageWriter.WriteBytesAndSize(VentsToList(vents).ToArray());
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void RpcSealVent(Vent vent) {
			if (AmongUsClient.Instance.AmHost)
				SealVent(vent);

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SealVent, SendOption.Reliable, -1);
			messageWriter.Write(vent.Id);
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void SealVent(Vent vent) {
			if (vent == null)
				return;

			Sprite staticSealdedVent = HelperSprite.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.StaticVentSealed.png", 150f);
			Sprite animatedSealeddVent = HelperSprite.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.AnimatedVentSealed.png", 150f);

			PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
			animator?.Stop();
			vent.myRend.sprite = animator == null ? staticSealdedVent : animatedSealeddVent;
			vent.name = "SealedVent_" + vent.name;
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
	}
}
