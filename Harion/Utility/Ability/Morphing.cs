using Harion.Data;
using Harion.Utility.Utils;
using System.Collections.Generic;

namespace Harion.Utility.Ability {
    public static class Morphing {

        private static List<PlayerControl> MorphPlayer = new();

        public static void Morph(PlayerControl Player, PlayerControl MorphedPlayer, bool resetAnim = false) {
            MorphPlayer.AddPlayer(Player);
            Player.RpcSetHat(MorphedPlayer.Data.HatId);
            Player.RpcSetSkin(MorphedPlayer.Data.SkinId);
            Player.RpcSetPet(MorphedPlayer.Data.PetId);
            Player.RpcSetColor((byte) MorphedPlayer.Data.ColorId);
            Player.RpcSetName(MorphedPlayer.Data.PlayerName);

            if (resetAnim && !Player.inVent)
                Player.MyPhysics.ResetAnimState();
        }

        public static void Unmorph(PlayerControl Player, bool resetAnim = false) {
            MorphPlayer.RemovePlayer(Player);
            InitialPlayerApparence PlayerData = InitialPlayerApparence.GetLocalPlayerData();
            Player.RpcSetHat(PlayerData.PlayerHat);
            Player.RpcSetSkin(PlayerData.PlayerSkin);
            Player.RpcSetPet(PlayerData.PlayerPet);
            Player.RpcSetColor((byte) PlayerData.PlayerColor);
            Player.RpcSetName(PlayerData.PlayerName);

            if (resetAnim && !Player.inVent)
                Player.MyPhysics.ResetAnimState();
        }

        public static bool IsMorphed(PlayerControl Player) => MorphPlayer.ContainsPlayer(Player);
    }
}
