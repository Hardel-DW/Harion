using InnerNet;

namespace Harion.Utility.Utils {
    public static class GameUtils {

        public static bool GameStarted => 
            GameData.Instance &&
            ShipStatus.Instance &&
            AmongUsClient.Instance &&
            (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started || AmongUsClient.Instance.GameMode == GameModes.FreePlay);
    }
}
