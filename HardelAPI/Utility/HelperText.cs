using TMPro;
using UnityEngine;

namespace HardelAPI.Utility {
    public static class HelperText {
        public static GameObject CreateTMP(this GameObject gameObject, string text, Vector2 Position, Color color, int size = 4) {
            GameObject tmpObject = new GameObject { name = "TMP_Count", layer = 5 };
            tmpObject.transform.SetParent(gameObject.transform);
            tmpObject.transform.position = Position;
            tmpObject.transform.localPosition = Position;

            MeshRenderer renderer = tmpObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("TextMeshPro/Mobile/Distance Field"));

            tmpObject.AddComponent<MeshFilter>();
            TextMeshPro textMeshPro = tmpObject.AddComponent<TextMeshPro>();
            textMeshPro.font = Resources.Load("ARIAL SDF") as TMP_FontAsset;
            textMeshPro.text = text;
            textMeshPro.color = color;
            textMeshPro.fontSize = size;

            return tmpObject;
        }
    }
}
