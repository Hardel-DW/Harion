namespace HardelAPI.Utility.Utils {
    public static class ServerUtils {

        public static bool IsCustomServer() {
            if (DestroyableSingleton<ServerManager>.Instance == null)
                return false;

            StringNames name = DestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
            return name != StringNames.ServerNA && name != StringNames.ServerEU && name != StringNames.ServerAS;
        }
    }
}
