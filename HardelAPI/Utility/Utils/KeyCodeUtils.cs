using UnityEngine;

namespace HardelAPI.Utility.Utils {
    public static class KeyCodeUtils {
        public static KeyCode KeyCodeFromString(string KeyCode) => (KeyCode) System.Enum.Parse(typeof(KeyCode), KeyCode);
    }
}
