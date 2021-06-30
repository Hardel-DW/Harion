using Harion.Reactor;
using Harion.Utility.Utils;
using System;
using System.IO;
using System.Linq;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Harion {
    [RegisterInIl2Cpp]
    public class HarionComponent : MonoBehaviour {

        private bool BlockFillDisable = false;

        [HideFromIl2Cpp]
        public HarionPlugin Plugin { get; internal set; }

        private void Start() {
            ModManager.Instance.ShowModStamp();
        }

        public HarionComponent(IntPtr ptr) : base(ptr) { }

        void Update() {
            if (!BlockFillDisable) {
                GameObject BlockFill = DestroyableSingleton<AccountManager>.Instance?.gameObject?.transform.Find("BackgroundFill/BlockFill").gameObject;
                if (BlockFill != null && BlockFill.scene.IsValid()) {
                    BlockFill.SetActive(false);
                    BlockFillDisable = true;
                    DestroyableSingleton<EOSManager>.Instance?.CheckAgeAndLoginStatus();
                }
            }
        }

        void OnApplicationQuit() {
            try {
                DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\plugins");
                string[] files = directory.GetFiles("*.old").Select(file => file.FullName).ToArray();
                foreach (var file in files)
                    File.Delete(file);
            } catch (Exception) {
                throw;
            }
        }
    }
}