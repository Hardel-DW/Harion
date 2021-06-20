using System;
using UnityEngine;

namespace HardelAPI.Utility {
    public static class Draw {
        public static void DrawCircle(LineRenderer lineRenderer, float thetaScale, float radius, UnityEngine.Color color, Vector3 position) {
            float Theta = 0f;
            int size = (int) ((1f / thetaScale) + 1f);
            lineRenderer.SetVertexCount(size);
            lineRenderer.material.color = Color.white;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;

            for (int i = 0; i < size; i++) {
                Theta += 2.0f * (float) Math.PI * thetaScale;
                float x = radius * Mathf.Cos(Theta);
                float y = radius * Mathf.Sin(Theta);
                lineRenderer.SetPosition(i, new Vector3(x + position.x, y + position.y, 0));
            }
        }
    }
}
