using Harion.Reactor;
using System;
using UnityEngine;

namespace Harion.ArrowManagement {

    [RegisterInIl2Cpp]
    public class UpdateComponent : MonoBehaviour {
        public ArrowBehaviour ArrowComponent;
        private float nextActionTime = 0.0f;
        public float period = 5f;

        public UpdateComponent(IntPtr ptr) : base(ptr) { }

        void Start() {
            ArrowComponent = gameObject.GetComponent<ArrowBehaviour>();
        }

        void Update() {
            if (period > 0f) {
                if (Time.time > nextActionTime) {
                    nextActionTime = Time.time + period;
                    ChangePositon();
                }
            } else {
                ChangePositon();
            }
        }

        void ChangePositon() {
            ArrowComponent.target = gameObject.transform.parent.position;
        }
    }
}
