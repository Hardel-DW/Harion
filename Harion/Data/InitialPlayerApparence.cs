using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.Data {
    public class InitialPlayerApparence {
        public static Dictionary<byte, InitialPlayerApparence> PlayersApparences = new();
        public int PlayerColor;
        public uint PlayerHat;
        public uint PlayerPet;
        public uint PlayerSkin;
        public string PlayerName;
        public Color PlayerColorName;

        public InitialPlayerApparence(PlayerControl Player) {
            PlayerColor = Player.Data.ColorId;
            PlayerHat = Player.Data.HatId;
            PlayerPet = Player.Data.PetId;
            PlayerSkin = Player.Data.SkinId;
            PlayerName = Player.Data.PlayerName;
            PlayerColorName = Player.nameText.color;
            PlayersApparences.Add(Player.PlayerId, this);
        }

        public static InitialPlayerApparence GetPlayerData(PlayerControl Player) =>
            PlayersApparences.FirstOrDefault(PIA => PIA.Key == Player.PlayerId).Value;

        public static InitialPlayerApparence GetLocalPlayerData() => GetPlayerData(PlayerControl.LocalPlayer);
    }
}
