using HardelAPI.Reactor;
using HardelAPI.Utility.Utils;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace HardelAPI {
    public static class ResourceLoader {
        private static readonly Assembly myAsembly = Assembly.GetExecutingAssembly();
        public static Material ArialFont;
        public static Material Liberia;

        public static void LoadAssets() {
            Stream resourceSteam = myAsembly.GetManifestResourceStream("HardelAPI.Resources.Harion");
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(resourceSteam.ReadFully());

            ArialFont = assetBundle.LoadAsset<Material>("ArialMasked.mat").DontDestroy();
            Liberia = assetBundle.LoadAsset<Material>("LiberationSans SDF - Mask.mat").DontDestroy();
        }
    }
}
