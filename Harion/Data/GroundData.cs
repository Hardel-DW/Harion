using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harion.Data {
    public class GroundData {

        public Vector2 OriginalPosition { get; } = new Vector2();
        public float OriginalRadius { get; } = 1f;
        public int NummberOfCloestElement { get; internal set; } = 0;
        public List<Collider2D> GameObjectsFounds { get; internal set; } = new();
        public bool GroundIsFound { get; internal set; } = false;
        public GameObject Ground { get; internal set; }

        public GroundData(Vector2 originalPosition, float originalRadius) {
            OriginalPosition = originalPosition;
            OriginalRadius = originalRadius;
        }

        public static GroundData GroundIsNearest(Vector2 Position, float radius) {
            GroundData groundData = new GroundData(Position, radius);
            GameObject ground = null;

            List<Collider2D> GameObjects = Physics2D.OverlapCircleAll(Position, radius).ToList();
            groundData.GameObjectsFounds = GameObjects;
            if (GameObjects.Count > 0) {
                groundData.NummberOfCloestElement = GameObjects.Count;
                foreach (Collider2D collider in GameObjects) {
                    GameObject go = collider.gameObject;
                    if (go.name == "Ground") {
                        groundData.Ground = go;
                        ground = go;
                        break;
                    }
                }
            }

            groundData.GroundIsFound = ground != null;
            return groundData;
        }
    }
}
