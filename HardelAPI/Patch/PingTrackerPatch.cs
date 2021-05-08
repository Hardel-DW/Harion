/*using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace HardelAPI.Patch {

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    [HarmonyPriority(Priority.First)]
    public static class PingTrackerPatch {
        private static Vector3 lastDist = Vector3.zero;

        public static void Postfix(ref PingTracker __instance) {
            if (!(AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)) {
                AspectPosition aspect = __instance.text.gameObject.GetComponent<AspectPosition>();
                if (aspect.DistanceFromEdge != lastDist) {
                    aspect.DistanceFromEdge += new Vector3(0.6f, 0);
                    aspect.AdjustPosition();

                    lastDist = aspect.DistanceFromEdge;
                }

                if (!AmongUsClient.Instance.AmHost || !GameData.Instance)
                    return;

                ShowLobbyCountdown.timer = Mathf.Max(0f, ShowLobbyCountdown.timer -= Time.deltaTime);
                int minutes = (int) ShowLobbyCountdown.timer / 60;
                int seconds = (int) ShowLobbyCountdown.timer % 60;
                string suffix = $" ({minutes:00}:{seconds:00})";

                __instance.text.text += $"\n{suffix}";
            }
        }
    }
}
*/