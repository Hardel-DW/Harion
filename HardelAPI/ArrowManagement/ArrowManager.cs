using HardelAPI.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI.ArrowManagement {
    public class ArrowManager {
        public static List<ArrowManager> allArrow = new List<ArrowManager>();

        public GameObject Parent { get; set; }
        public GameObject Arrow { get; set; }
        public ArrowBehaviour ArrowComponent { get; set; }
        public SpriteRenderer Renderer { get; set; }
        public bool IsDynamic { get; set; }
        public float UpdateInterval { get; set; }

        public ArrowManager(GameObject Parent, Vector2 Position, bool IsDynamic = false, float Time = 1f) {
            this.Parent = Parent;
            this.IsDynamic = IsDynamic;
            this.UpdateInterval = Time;

            CreateArrow(Parent, Position, IsDynamic, Time);
        }

        public ArrowManager(GameObject Parent, bool IsDynamic = false, float Time = 1f) {
            this.Parent = Parent;
            this.IsDynamic = IsDynamic;
            this.UpdateInterval = Time;

            CreateArrow(Parent, Parent.transform.position, IsDynamic, Time);
        }

        private void CreateArrow(GameObject Parent, Vector2 Position, bool IsDynamic, float Time) {
            Arrow = new GameObject { layer = 5, name = "Arrow" };
            Arrow.transform.SetParent(Parent.transform);
            Arrow.transform.position = Parent.transform.position;
            ArrowComponent = Arrow.AddComponent<ArrowBehaviour>();
            Renderer = Arrow.AddComponent<SpriteRenderer>();
            ArrowComponent.image = Renderer;
            ArrowComponent.target = Parent.transform.position;
            Renderer.sprite = HelperSprite.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Arrow.png", 150f);
            Arrow.AddComponent<UpdateComponent>().period = Time;

            allArrow.Add(this);
            Arrow.SetActive(true);
        }

        public Vector2 GetArrowPosition() {
            return ArrowComponent.target;
        }
    }
}