using System;
using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI.Data {
    public class DangerPoint {
        public static List<DangerPoint> points = new List<DangerPoint>();

        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Sprite Texture { get; set; }
        public string TypeElement { get; set; }
        public bool SaboatageHidePoint { get; set; }
        public GameObject GameObject { get; set; }
        public DateTime? ExpiredOn { get; set; }

        public DangerPoint(Vector2 position, UnityEngine.Color color, string typeElement, bool saboatageHidePoint = false, Sprite texture = null, DateTime? ExpiredOn = null) {
            this.Position = position;
            this.Color = color;
            this.Texture = texture;
            this.TypeElement = typeElement;
            this.SaboatageHidePoint = saboatageHidePoint;

            if (ExpiredOn == null)
                ExpiredOn = DateTime.MaxValue;
            else
                this.ExpiredOn = ExpiredOn;

            this.GameObject = CreateGameObject();
            points.Add(this);
        }

        private GameObject CreateGameObject() {
            if (MapBehaviour.Instance == null) {
                DestroyableSingleton<HudManager>.Instance.ShowMap((Action<MapBehaviour>) (map => {
                    map.gameObject.SetActive(false);
                    map.HerePoint.enabled = true;
                }));
            }

            GameObject Prefab = MapBehaviour.Instance.gameObject.transform.Find("HereIndicatorParent/TaskOverlay/TaskIcon(Clone)").gameObject;
            GameObject PooledMapIcon = UnityEngine.Object.Instantiate(Prefab, Prefab.transform.parent);
            SpriteRenderer renderer = PooledMapIcon.GetComponent<SpriteRenderer>();
            if (Texture != null)
                renderer.sprite = Texture;

            renderer.color = Color;
            renderer.material.SetFloat("_Outline", 1f);
            renderer.material.SetColor("_OutlineColor", Color);
            PooledMapIcon.layer = 5;
            PooledMapIcon.transform.localScale = Vector3.one;

            Vector3 vector = Position;
            vector /= ShipStatus.Instance.MapScale;
            vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
            vector.z = -1f;
            PooledMapIcon.name = $"Danger Point {TypeElement}";
            PooledMapIcon.transform.localPosition = vector;

            return PooledMapIcon;
        }
    }
}
