using HardelAPI.Reactor;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace HardelAPI {
    [RegisterInIl2Cpp]
    public class HarionComponent : MonoBehaviour {
        [HideFromIl2Cpp]
        public HardelApiPlugin Plugin { get; internal set; }

        public HarionComponent(IntPtr ptr) : base(ptr) { }
    } 
}