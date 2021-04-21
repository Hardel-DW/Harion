using Reactor;
using System;
using UnityEngine;

namespace HardelAPI.ArrowManagement {

    [RegisterInIl2Cpp]
    public class UpdateComponent : MonoBehaviour {

        public ArrowBehaviour ArrowComponent;

        public UpdateComponent(IntPtr ptr) : base(ptr) { }

        void Start() {
            ArrowComponent = gameObject.GetComponent<ArrowBehaviour>();
        }

        void Update() {
            ArrowComponent.target = gameObject.transform.parent.position;
        }
    }
}
