/*using Harion.Reactor;
using Hazel;
using System;
using UnityEngine;

namespace Harion.Utility {

    [RegisterInIl2Cpp]
    public class SyncroObject : MonoBehaviour {

        public SyncroObject(IntPtr ptr) : base(ptr) { }

        public bool AmOwner {
            get => OwnerId == AmongUsClient.Instance.ClientId;
        }

        public uint NetId;
        public SendOption sendMode = SendOption.None;
        public bool SyncroPlacement = true;
        public bool SyncroDestroy = true;
        public int OwnerId;

        public void UpdateObject() {

        }

        public string GetUniqId() => Guid.NewGuid().ToString("N");

        public void OnDisable() {

        }

        public void OnEnable() {

        }

        public void OnDestroy() {

        }

        public void Start() {

        }

    }
}
*/