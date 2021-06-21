using Hazel;
using UnityEngine;

namespace Harion.Utility.Ability {
    public static class Morphing {

        public static void Morph(PlayerControl Player, PlayerControl MorphedPlayer, bool resetAnim = false) {
            Player.RpcSetHat(MorphedPlayer.Data.HatId);
            Player.RpcSetSkin(MorphedPlayer.Data.SkinId);
            Player.RpcSetPet(MorphedPlayer.Data.PetId);
            Player.RpcSetColor((byte) MorphedPlayer.Data.ColorId);
            Player.RpcSetName(MorphedPlayer.Data.PlayerName);

            if (resetAnim && !Player.inVent)
                Player.MyPhysics.ResetAnimState();
        }

        public static void Unmorph(PlayerControl Player, bool resetAnim = false) {
            Player.RpcSetHat(Data.InitialPlayerApparence.PlayerHat);
            Player.RpcSetSkin(Data.InitialPlayerApparence.PlayerSkin);
            Player.RpcSetPet(Data.InitialPlayerApparence.PlayerPet);
            Player.RpcSetColor((byte) Data.InitialPlayerApparence.PlayerColor);
            Player.RpcSetName(Data.InitialPlayerApparence.PlayerName);

            if (resetAnim && !Player.inVent)
                Player.MyPhysics.ResetAnimState();
        }
    }
}
