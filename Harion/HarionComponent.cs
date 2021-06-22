using Harion.Reactor;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace Harion {
    [RegisterInIl2Cpp]
    public class HarionComponent : MonoBehaviour {
        [HideFromIl2Cpp]
        public HarionPlugin Plugin { get; internal set; }

        private void Start() => ModManager.Instance.ShowModStamp();

        public HarionComponent(IntPtr ptr) : base(ptr) { }
    } 
}