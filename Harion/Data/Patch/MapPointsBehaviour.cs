using HarmonyLib;
using System;

namespace Harion.Data.Patch {

    [HarmonyPatch]
    public class MapPointsBehaviour {

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.GenericShow))]
        public static class MapPointsBehaviourShow {
            public static void Postfix(MapBehaviour __instance) {
                CheckExpiredDate();
                DangerPoint.points.ForEach(dangerPoint => dangerPoint.GameObject.SetActive(true));
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
        public static class MapPointsBehaviourClose {
            public static void Postfix(MapBehaviour __instance) {
                DangerPoint.points.ForEach(dangerPoint => dangerPoint.GameObject.SetActive(false));
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
        public static class MapPointsBehaviourUpdate {
            public static void Postfix(MapBehaviour __instance) {
                CheckExpiredDate();
            }
        }

        private static void CheckExpiredDate() {
            foreach (var point in DangerPoint.points) {
                if (point.ExpiredOn != null) {
                    if (point.ExpiredOn < DateTime.Now) {
                        UnityEngine.Object.Destroy(point.GameObject);
                        DangerPoint.points.Remove(point);
                    }
                }
            }
        }
    }
}
