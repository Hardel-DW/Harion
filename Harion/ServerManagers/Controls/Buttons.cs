using Harion.Utility.Utils;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Harion.ServerManagers.Controls {
    public class Button {
        private static readonly TaskCompletionSource<bool> Initialized = new TaskCompletionSource<bool>();
        private static GameObject Instance;
        private readonly SpriteRenderer SpriteRenderer;
        private readonly BoxCollider2D BoxCollider2D;

        public GameObject GameObject { get; }

        public TextMeshPro Text { get; }
        
        public Vector2 Size {
            get => SpriteRenderer.size;
            set {
                Text.rectTransform.sizeDelta = value;
                SpriteRenderer.size = value;
                BoxCollider2D.size = value;
            }
        }

        public AspectPosition Position { get; }

        public UnityEngine.UI.Button.ButtonClickedEvent OnClick { get; }

        private Button(string text) {
            GameObject = Object.Instantiate(Instance).DontDestroy();
            Text = GameObject.GetComponentInChildren<TextMeshPro>();
            Position = GameObject.GetComponent<AspectPosition>();
            OnClick = GameObject.GetComponent<PassiveButton>().OnClick;
            SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            BoxCollider2D = GameObject.GetComponent<BoxCollider2D>();

            Vector2 size = new Vector2(Text.preferredWidth, Text.preferredHeight);

            Text.text = text;
            Text.rectTransform.sizeDelta = size;
            Position.DistanceFromEdge =
                new Vector3(Text.preferredWidth / 2f + 0.1f, Text.preferredHeight / 2f + 0.1f, -100f);
            SpriteRenderer.size = size;
            BoxCollider2D.size = size;
        }

        public static async Task<Button> Create(string text) {
            await Initialized.Task;

            return new Button(text);
        }

        public Button SetText(string newText) {
            Text.text = newText;
            return this;
        }

        public Button SetSize(Vector2 newSize) {
            Size = newSize;
            return this;
        }

        public Button SetDistanceFromEdge(Vector2 newPosition) {
            Position.DistanceFromEdge = new Vector3(newPosition.x, newPosition.y, -100f);
            return this;
        }

        public Button SetAlignment(AspectPosition.EdgeAlignments newAlignment) {
            Position.Alignment = newAlignment;
            return this;
        }

        public static void InitializeBaseButton() {
            BaseButton.Initialize();
        }

        private static class BaseButton {
            public static void Initialize() {
                SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) OnSceneLoaded);
            }

            private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
                Instantiate();

                SceneManager.remove_sceneLoaded((Action<Scene, LoadSceneMode>) OnSceneLoaded);
            }

            private static void Instantiate() {
                if (Initialized.Task.IsCompleted)
                    return;

                var exitButton = GameObject.Find("ExitGameButton");
                if (!exitButton)
                    return;

                Instance = Object.Instantiate(exitButton).DontDestroy();
                Instance.SetActive(false);
                Instance.name = "Button";

                Instance.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                Instance.GetComponent<SceneChanger>().Destroy();
                Instance.GetComponent<ConditionalHide>().Destroy();
                Instance.GetComponentInChildren<TextTranslatorTMP>().Destroy();
                Instance.GetComponentInChildren<TextMeshPro>().text = "Button";

                Initialized.SetResult(true);
            }
        }
    }
}
