using Hazel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HardelAPI.Utility.Utils;
using System.Linq;
using HardelAPI.Reactor;

namespace HardelAPI.Utility.Ability {
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
            Color color = Color.clear;
            if (PlayerControl.LocalPlayer.PlayerId == Player.PlayerId || PlayerControl.LocalPlayer.Data.IsDead)
                color.a = 0.1f;

            foreach (PlayerControl whiteListPlayer in whiteListVisibility)
                if (whiteListPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    color.a = 0.1f;

            Player.GetComponent<SpriteRenderer>().color = color;
            Player.HatRenderer.SetHat(0, 0);
            Player.nameText.text = "";

            if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance.AllSkins.ToArray()[0].ProdId)
                Player.MyPhysics.SetSkin(0);

            if (Player.CurrentPet != null)
                Object.Destroy(Player.CurrentPet.gameObject);

            Player.CurrentPet = Object.Instantiate(DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0]);
            Player.CurrentPet.transform.position = Player.transform.position;
            Player.CurrentPet.Source = Player;
            Player.CurrentPet.Visible = Player.Visible;

            yield return new WaitForSeconds(Duration);
            Player.GetComponent<SpriteRenderer>().color = Color.white;
            yield return true;
        }
    }
}
