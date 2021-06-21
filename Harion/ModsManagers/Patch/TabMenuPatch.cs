using HarmonyLib;
using UnityEngine;

namespace Harion.ModsManagers.Patch {

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class TabMenuPatch {

        public static void Prefix() {
            GameObject AccountManager = GameObject.Find("AccountManager");
            GameObject AccountTab = AccountManager.transform.Find("AccountTab/AccountTab").gameObject;
            AccountTab.transform.localPosition = new Vector3(AccountTab.transform.localPosition.x, 0f, AccountTab.transform.localPosition.z);
        }
    }
}