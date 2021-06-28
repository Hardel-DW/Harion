using HarmonyLib;
using UnityEngine;
using Harion.Utility.Utils;

namespace Harion.Patch {

    [HarmonyPatch(typeof(AccountManager), nameof(AccountManager.Awake))]
    public static class SteamPatch {

        public static void Prefix(AccountManager __instance) {
            Object.Destroy(__instance.gameObject.FindObject("BackgroundFill"));
        }
    }
}
