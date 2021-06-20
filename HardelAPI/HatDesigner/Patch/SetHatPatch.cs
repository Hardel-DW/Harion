using HarmonyLib;
using UnityEngine;

namespace HardelAPI.HatDesigner.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetHat))]
    public static class SetHatPatch {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] uint hatId) {
            __instance.nameText.transform.localPosition = new Vector3(
                0f,
                hatId == 0U ? 0.7f : HatCreator.TallIds.Contains(hatId) ? 1.2f : 1.05f,
                -0.5f
            );
        }
    }
}
