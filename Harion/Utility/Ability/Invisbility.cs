using Hazel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harion.Utility.Utils;
using Harion.Reactor;
using Harion.Utility.Helper;

namespace Harion.Utility.Ability {
    public static class Invisbility {

        private static List<PlayerControl> InvisiblePlayer = new();

        public static void LaunchInvisibility(PlayerControl Player, float Duration, List<PlayerControl> whiteListVisibility = null) {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Invisibility, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(Duration);
            writer.WriteBytesAndSize(PlayerControlUtils.PlayerControlListToIdList(whiteListVisibility).ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            Coroutines.Start(Invisibility(Player, Duration, whiteListVisibility));
        }

        internal static IEnumerator Invisibility(PlayerControl Player, float Duration, List<PlayerControl> whiteListVisibility = null) {
            float alpha = 0f;
            if (PlayerControl.LocalPlayer.PlayerId == Player.PlayerId || PlayerControl.LocalPlayer.Data.IsDead)
                alpha = 0.2f;

            foreach (PlayerControl whiteListPlayer in whiteListVisibility)
                if (whiteListPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    alpha = 0.2f;

            Invisibility(Player, alpha);
            yield return new WaitForSeconds(Duration);
            Invisibility(Player, 1f);
            yield return true;
        }

        private static void Invisibility(PlayerControl Player, float alpha) {
            Player.myRend.SetColorAlpha(alpha);
            Player.nameText.enabled = alpha <= 0 ? false : true;

            if (alpha >= 1f) {
                InvisiblePlayer.RemovePlayer(Player);
                Player.HatRenderer.BackLayer.enabled = true;
                Player.HatRenderer.FrontLayer.enabled = true;
            }
            else {
                Player.HatRenderer.BackLayer.enabled = false;
                Player.HatRenderer.FrontLayer.enabled = false;
                InvisiblePlayer.AddPlayer(Player);
            }

            if (Player.MyPhysics != null && Player.MyPhysics.Skin != null) {
                Player.MyPhysics.Skin.layer.SetColorAlpha(alpha);
            }

            if (Player.CurrentPet != null) {
                Player.CurrentPet.rend.SetColorAlpha(alpha);

                if (Player.CurrentPet.shadowRend != null) {
                    Player.CurrentPet.shadowRend.SetColorAlpha(alpha);
                }
            }
        }

        public static void StopInvisibility(PlayerControl Player) => Invisibility(Player, 1f);

        public static bool IsInvisible(PlayerControl Player) => InvisiblePlayer.ContainsPlayer(Player);
    }
}
