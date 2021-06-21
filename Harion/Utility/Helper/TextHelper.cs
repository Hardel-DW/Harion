using TMPro;
using UnhollowerBaseLib;
using UnityEngine;

namespace Harion.Utility.Helper {
    public static class TextHelper {

        public static GameObject CreateTMP(this GameObject gameObject, string text, string GameObjectName) {
            GameObject tmpObject = new GameObject { name = GameObjectName, layer = 5 };
            tmpObject.transform.SetParent(gameObject.transform);
            
            MeshRenderer renderer = tmpObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("TextMeshPro/Mobile/Distance Field"));

            tmpObject.AddComponent<MeshFilter>();
            TextMeshPro textMeshPro = tmpObject.AddComponent<TextMeshPro>();
            textMeshPro.font = Resources.Load("ARIAL SDF") as TMP_FontAsset;
            textMeshPro.text = text;
            
            return tmpObject;
        }

        public static GameObject CreateTMP(this GameObject gameObject, string text, Vector2 Position, UnityEngine.Color color, int size = 4) {
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

        /// <summary>
        /// Gets translated strings from <see cref="StringNames"/> (an instance of <see cref="TranslationController"/> has to exist).
        /// </summary>
        /// <param name="str">String name to retrieve</param>
        /// <param name="parts">Elements to pass for formatting</param>
        /// <returns>The translated value of <see cref="str"/></returns>
        public static string GetText(this StringNames str, params object[] parts) => DestroyableSingleton<TranslationController>.Instance?.GetString(str, (Il2CppReferenceArray<Il2CppSystem.Object>) parts) ?? "STRMISS";
    }
}
