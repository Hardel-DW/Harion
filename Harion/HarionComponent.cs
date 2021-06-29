using Harion.Reactor;
using System;
using System.IO;
using System.Linq;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Harion {
    [RegisterInIl2Cpp]
    public class HarionComponent : MonoBehaviour {
        [HideFromIl2Cpp]
        public HarionPlugin Plugin { get; internal set; }

        private void Start() => ModManager.Instance.ShowModStamp();

        public HarionComponent(IntPtr ptr) : base(ptr) { }

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