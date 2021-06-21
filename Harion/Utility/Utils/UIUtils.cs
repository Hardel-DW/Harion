using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Harion.Utility.Utils {
    public static class UIUtils {
        /// <summary>
        /// Shortcut for empty texture
        /// </summary>
        public static Texture2D CreateEmptyTexture(int width = 0, int height = 0) {
            return new Texture2D(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);
        }

        /// <summary>
        /// Clamp Rect to screen size
        /// </summary>
        public static Rect ClampScreen(this Rect rect) {
            rect.x = Mathf.Clamp(rect.x, 0, Screen.width - rect.width);
            rect.y = Mathf.Clamp(rect.y, 0, Screen.height - rect.height);

            return rect;
        }

        /// <summary>
        /// Reset Rect size
        /// </summary>
        public static Rect ResetSize(this Rect rect) {
            rect.width = rect.height = 0;

            return rect;
        }

        /// <summary>
        /// Create <see cref="Sprite"/> from <paramref name="tex"/>
        /// </summary>
        public static Sprite CreateSprite(this Texture2D tex) {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }

        public static DefaultControls.Resources StandardResources { get; internal set; }

        public static GameObject CreateCanvas() {
            var gameObject = new GameObject("Canvas");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = gameObject.AddComponent<CanvasScaler>();

            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.referencePixelsPerUnit = 100f;

            gameObject.AddComponent<GraphicRaycaster>();

            return gameObject;
        }

        public static GameObject CreateEventSystem() {
            var gameObject = new GameObject("EventSystem");
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
            gameObject.AddComponent<BaseInput>();

            return gameObject;
        }

        public static void SetSize(this RectTransform rectTransform, float width, float height) {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}
