using Harion.Reactor;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Harion.Utility.Utils {
    public static class DeadBodyUtils {

        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");

        public static void CleanBodyDuration(DeadBody body, float duration) {
            Coroutines.Start(CleanCoroutine(body, duration));

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.CleanBody, SendOption.Reliable, -1);
            writer.Write(body.ParentId);
            writer.Write(duration);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        internal static IEnumerator CleanCoroutine(DeadBody body, float duration) {
            SpriteRenderer renderer = body.GetComponent<SpriteRenderer>();
            Color backColor = renderer.material.GetColor(BackColor);
            Color bodyColor = renderer.material.GetColor(BodyColor);
            Color newColor = new Color(1f, 1f, 1f, 0f);
            for (var i = 0; i < duration; i++) {
                if (body == null)
                    yield break;

                renderer.color = Color.Lerp(backColor, newColor, i / duration);
                renderer.color = Color.Lerp(bodyColor, newColor, i / duration);
                yield return null;
            }

            Object.Destroy(body.gameObject);
        }

        public static DeadBody FromParentId(byte id) => Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == id);
        
        public static void CleanBody(PlayerControl player) => CleanBody(player.PlayerId);
        public static void CleanBody(byte playerId) => Object.Destroy(Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == playerId).gameObject);
    }
}
