using Harion.Data;

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
            InitialPlayerApparence PlayerData = InitialPlayerApparence.GetLocalPlayerData();
            Player.RpcSetHat(PlayerData.PlayerHat);
            Player.RpcSetSkin(PlayerData.PlayerSkin);
            Player.RpcSetPet(PlayerData.PlayerPet);
            Player.RpcSetColor((byte) PlayerData.PlayerColor);
            Player.RpcSetName(PlayerData.PlayerName);

            if (resetAnim && !Player.inVent)
                Player.MyPhysics.ResetAnimState();
        }
    }
}
