using HarmonyLib;

namespace HardelAPI.Data.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public static class ShipStatusStart {
        public static void Postfix(PlayerControl __instance) {
            DeadPlayer.ClearDeadPlayer();

            InitialPlayerApparence.PlayerHat = __instance.Data.HatId;
            InitialPlayerApparence.PlayerPet = __instance.Data.PetId;
            InitialPlayerApparence.PlayerSkin = __instance.Data.SkinId;
            InitialPlayerApparence.PlayerColor = __instance.Data.ColorId;
            InitialPlayerApparence.PlayerColorName = __instance.nameText.color;
            InitialPlayerApparence.PlayerName = __instance.Data.PlayerName;
            InitialPlayerApparence.PlayerHat = __instance.Data.HatId;
        }
    }
}
