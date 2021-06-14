using HardelAPI.Data;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardelAPI.Utility.Utils {
    public static class CameraUtils {

		public static void RpcAddMutipleCamera(List<Vector2> positions) {
			foreach (var position in positions)
				AddNewCamera(position);

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.PlaceCameraBuffer, SendOption.Reliable, -1);
			messageWriter.WriteListVector2(positions);
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void RpcAddCamera(Vector2 position) {
			AddNewCamera(position);

			MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.PlaceCamera, SendOption.Reliable, -1);
			messageWriter.WriteVector2(position);
			AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
		}

		public static void AddNewCamera(Vector3 position) {
			SurvCamera referenceCamera = Object.FindObjectOfType<SurvCamera>();
			if (referenceCamera == null)
				return;

			SurvCamera camera = Object.Instantiate<SurvCamera>(referenceCamera);
			camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
			camera.CamName = $"Custom Camera";
			camera.Offset = new Vector3(0f, 0f, camera.Offset.z);

			if (PlayerControl.GameOptions.MapId == 2 || PlayerControl.GameOptions.MapId == 4)
				camera.transform.localRotation = new Quaternion(0, 0, 1, 1);

			camera.gameObject.SetActive(true);

			List<SurvCamera> allCameras = ShipStatus.Instance.AllCameras.ToList();
			allCameras.Add(camera);
			ShipStatus.Instance.AllCameras = allCameras.ToArray();

			new DangerPoint(camera.transform.position, Palette.ImpostorRed, "Camera Custom");
		}

		public static void DeleteCustomCamera() {
			if (ShipStatus.Instance == null)
				return;

            foreach (SurvCamera camera in ShipStatus.Instance.AllCameras) {
				if (camera.CamName.StartsWith("Custom")) {
					List<SurvCamera> allCameras = ShipStatus.Instance.AllCameras.ToList();
					allCameras.Remove(camera);
					ShipStatus.Instance.AllCameras = allCameras.ToArray();

					Object.Destroy(camera);
				}
			}
		}
	}
}
