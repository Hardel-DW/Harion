using HarmonyLib;
using System;

namespace Harion.HatDesigner.Patch {

    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
    class GetHatPatch {
        static bool Prefix(HatManager __instance) {
            try {
                if (!HatCreator.modded) {
                    HatCreator.modded = true;

                    for (int i = 0; i < HatCreator.allHatsData.Count; i++) {
                        CustomHat hatData = HatCreator.allHatsData[i];
                        HatBehaviour hat = HatCreator.CreateHat(hatData, i);
                        __instance.AllHats.Add(hat);
                        if (hatData.highUp)
                            HatCreator.TallIds.Add((uint) (__instance.AllHats.Count - 1));

                        HatCreator.IdToData.Add((uint) __instance.AllHats.Count - 1, hatData);
                    }
                }
                return true;
            } catch (Exception e) {
                HarionPlugin.Logger.LogWarning("Attention an error was caught in the class GetHatPatch, in the Prefix : " + e);
                throw;
            }
        }
    }
}
