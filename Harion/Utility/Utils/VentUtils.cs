using Harion.Data;
using Harion.Utility.Helper;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.Utility.Utils {

    public static class VentUtils {
		private static List<Vent> listVent = new List<Vent>();
		private static Vent lastVent;

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
			HarionPlugin.Logger.LogInfo($"ventIds: {vents.Count}");
			foreach (var vent in vents) {
				HarionPlugin.Logger.LogInfo($"ventId: {vent.Id}, ventsToSeal: {vent.name}");

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

			HarionPlugin.Logger.LogInfo($"Seal vent:  {vent.Id}");

			Sprite staticSealdedVent = SpriteHelper.LoadSpriteFromEmbeddedResources("Harion.Resources.StaticVentSealed.png", 150f);
			Sprite animatedSealeddVent = SpriteHelper.LoadSpriteFromEmbeddedResources("Harion.Resources.AnimatedVentSealed.png", 150f);

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

		public static Vent PlaceVent(Vector3 Position) {
			int ventId = GetAvailableVentId();
			int ventLeft = int.MaxValue;
			int ventCrnter = int.MaxValue;
			int ventRight = int.MaxValue;

			if (lastVent != null)
				ventLeft = lastVent.Id;

			Vent vent = RpcSpawnVent(ventId, Position, ventLeft, ventCrnter, ventRight);
			return vent;
		}

		private static int GetAvailableVentId() {
			int id = 0;

			while (true) {
				if (!ShipStatus.Instance.AllVents.Any(v => v.Id == id))
					return id;

				id++;
			}
		}

		internal static Vent SpawnVent(int id, Vector3 postion, int leftVent, int centerVent, int rightVent) {
			Vent ventPref = GameObject.FindObjectOfType<Vent>();
			Vent vent = GameObject.Instantiate<Vent>(ventPref, ventPref.transform.parent);

			vent.Id = id;
			vent.transform.position = new Vector3(postion.x, postion.y, postion.z + 0.1f);
			vent.Left = leftVent == int.MaxValue ? null : ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == leftVent);
			vent.Center = centerVent == int.MaxValue ? null : ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == centerVent);
			vent.Right = rightVent == int.MaxValue ? null : ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == rightVent);

            List<Vent> allVents = ShipStatus.Instance.AllVents.ToList();
			allVents.Add(vent);
			ShipStatus.Instance.AllVents = allVents.ToArray();

			if (lastVent != null)
				lastVent.Right = ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == id);

			lastVent = vent;
			return vent;
		}

		private static Vent RpcSpawnVent(int id, Vector3 postion, int leftVent, int centerVent, int rightVent) {
			Vent vent = SpawnVent(id, postion, leftVent, centerVent, rightVent);
			MessageWriter w = AmongUsClient.Instance.StartRpc(ShipStatus.Instance.NetId, (byte) CustomRPC.PlaceVent, SendOption.Reliable);

			w.WritePacked(id);
			w.WriteVector3(postion);
			w.WritePacked(leftVent);
			w.WritePacked(centerVent);
			w.WritePacked(rightVent);
			w.EndMessage();

			return vent;
		}
	}
}