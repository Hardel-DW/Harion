using Hazel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harion.Utility.Utils;
using Harion.Reactor;
using Harion.Utility.Helper;

namespace Harion.Utility.Ability {
    public static class Invisbility {

        private static Dictionary<PlayerControl, float> InvisiblePlayer = new();

        public static void RpcLaunchInvisibility(PlayerControl Player, float Duration, List<PlayerControl> whiteListVisibility = null) {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Invisibility, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(Duration);
            writer.WriteBytesAndSize(PlayerControlUtils.PlayerControlListToIdList(whiteListVisibility).ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            LaunchInvisibility(Player, Duration, whiteListVisibility);
        }

        internal static IEnumerator Invisibility(PlayerControl Player, float Duration, float alpha) {
            Invisibility(Player, alpha);
            yield return new WaitForSeconds(Duration);
            Invisibility(Player, 1f);
            yield return true;
        }

        public static void LaunchInvisibility(PlayerControl Player, float? Duration = null, List<PlayerControl> whiteList = null) {
            float alpha = 0f;
            if (PlayerControl.LocalPlayer.PlayerId == Player.PlayerId || PlayerControl.LocalPlayer.Data.IsDead)
                alpha = 0.2f;

            foreach (PlayerControl whiteListPlayer in whiteList)
                if (whiteListPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    alpha = 0.2f;

            if (Duration == null)
                Invisibility(Player, alpha);
            else
                Coroutines.Start(Invisibility(Player, Duration.Value, alpha));
        }

        public static void RefreshInvisibility(PlayerControl Player) {
            if (Player == null)
                return;

            if (!InvisiblePlayer.ContainsPlayer(Player))
                return;

            float alpha = InvisiblePlayer.GetValueFromPlayer(Player);
            Invisibility(Player, alpha);
        }

        internal static void Invisibility(PlayerControl Player, float alpha) {
            Player.myRend.SetColorAlpha(alpha);
            Player.nameText.enabled = alpha <= 0 ? false : true;

            if (alpha >= 1f) {
                if (InvisiblePlayer.ContainsPlayer(Player))
                    InvisiblePlayer.RemovePlayer(Player);

                Player.HatRenderer.BackLayer.enabled = true;
                Player.HatRenderer.FrontLayer.enabled = true;
            } else {
                Player.HatRenderer.BackLayer.enabled = false;
                Player.HatRenderer.FrontLayer.enabled = false;

                if (!InvisiblePlayer.ContainsPlayer(Player))
                    InvisiblePlayer.Add(Player, alpha);
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
