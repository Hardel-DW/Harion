﻿using Hazel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harion.Utility.Utils;
using Harion.Reactor;

namespace Harion.Utility.Ability {
    public static class Invisbility {

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

            if (Player.HatRenderer != null) {
                Player.HatRenderer.FrontLayer.SetColorAlpha(alpha);
                Player.HatRenderer.BackLayer.SetColorAlpha(alpha);
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

        private static void SetColorAlpha(this SpriteRenderer renderer, float alpha) {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        }
    }
}