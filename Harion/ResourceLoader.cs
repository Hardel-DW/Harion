using Harion.Reactor;
using Harion.Utility.Utils;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Harion {
    public static class ResourceLoader {
        private static readonly Assembly myAsembly = Assembly.GetExecutingAssembly();
        public static Material ArialFont;
        public static Material Liberia;
        public static GameObject NavigationObject;

        public static void LoadAssets() {
            Stream resourceSteam = myAsembly.GetManifestResourceStream("Harion.Resources.Harion");
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(resourceSteam.ReadFully());

            ArialFont = assetBundle.LoadAsset<Material>("ArialMasked.mat").DontDestroy();
            Liberia = assetBundle.LoadAsset<Material>("LiberationSans SDF - Mask.mat").DontDestroy();
            NavigationObject = assetBundle.LoadAsset<GameObject>("NavigationMesh.prefab").DontDestroy();
        }
    }
}
