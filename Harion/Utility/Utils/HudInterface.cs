using UnityEngine;

namespace Harion.Utility.Utils {
    public static class HudInterface {

        internal static void ToggleTab() {
            GameObject taskStuff = DestroyableSingleton<HudManager>.Instance?.TaskStuff;
            if (taskStuff == null || !taskStuff.active)
                return;

            Component buttonObject = GameObjectUtils.GetChildComponentByName<Component>(taskStuff, "Tab");
            if (buttonObject == null)
                return;

            PassiveButton button = buttonObject.gameObject.GetComponent<PassiveButton>();
            button?.ReceiveClickDown();
        }
    }
}
