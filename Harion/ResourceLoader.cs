using Harion.Reactor;
using Harion.Utility.Helper;
using Harion.Utility.Utils;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Harion {
    public static class ResourceLoader {
        private static readonly Assembly myAsembly = Assembly.GetExecutingAssembly();
        public static Material Liberia;
        public static Material LiberiaShadow;
        public static TMP_FontAsset FontLiberation;
        public static Sprite RedCross;

        public static void LoadAssets() {
            Stream resourceSteam = myAsembly.GetManifestResourceStream("Harion.Resources.Harion");
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(resourceSteam.ReadFully());

            Liberia = assetBundle.LoadAsset<Material>("LiberationSans SDF - Mask.mat").DontDestroy();
            LiberiaShadow = assetBundle.LoadAsset<Material>("LiberationSans SDF - Drop Shadow").DontDestroy();
            FontLiberation = assetBundle.LoadAsset<TMP_FontAsset>("LiberationSans SDF.asset").DontDestroy();
            RedCross = SpriteHelper.LoadSpriteFromEmbeddedResources("Harion.Resources.RedCross.png", 128f).DontDestroy();
        }
    }
}
