using HardelAPI.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace HardelAPI.CustomKeyBinds.Components {
    public class PopUpWindow {
        private static readonly List<PopUpWindow> Windows = new List<PopUpWindow>();
        private Component backgroundComponent;
        private Component controlText;
        private TextMeshPro textRenderer;
        protected Component closeButton;
        protected GameObject holder;
        internal bool IsActive = false; 

        public string name;
        public Vector2 pos;
        public Vector2 size;
        public string title;

        public PopUpWindow(OptionsMenuBehaviour optionsMenu, string name, string title, Vector2 size, Vector2 pos, GameObject parent = null) {
            this.name = name;
            this.title = title;
            this.size = size;
            this.pos = pos;

            Windows.Add(this);
            Start(optionsMenu, parent);
        }

        private void Start(OptionsMenuBehaviour optionsMenu, GameObject parent) {
            //Get original components
            Component oBackgroundComponent = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "Background");
            Component oCloseButton = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "CloseButton");
            Component oControlText = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "ControlText_TMP");

            //Create component and add it to the parent
            holder = new GameObject(name);

            //Set button parent
            holder.transform.parent = parent == null ? optionsMenu.transform : parent.transform;

            //Configure holder and his components
            holder.layer = 5; //UI
            holder.transform.localScale = new Vector3(1f, 1f, 1f);
            holder.transform.localPosition = Vector3.back * 20f;

            //clone original sub components
            backgroundComponent = Object.Instantiate(oBackgroundComponent, holder.transform);
            if (oCloseButton != null)
                closeButton = Object.Instantiate(oCloseButton, holder.transform);
            controlText = Object.Instantiate(oControlText, holder.transform);

            //Configure background size
            backgroundComponent.transform.localPosition = new Vector3(backgroundComponent.transform.localPosition.x, backgroundComponent.transform.localPosition.y, 9f);
            SpriteRenderer renderer = backgroundComponent.gameObject.GetComponent<SpriteRenderer>();
            renderer.size = size;
            BoxCollider2D collider = backgroundComponent.gameObject.GetComponent<BoxCollider2D>();
            collider.size = size;

            //Configure close button
            if (closeButton != null) {
                closeButton.transform.localPosition = new Vector3(pos.x, pos.y);
                PassiveButton button = closeButton.gameObject.GetComponent<PassiveButton>();
                Object.Destroy(button);
                button = closeButton.gameObject.AddComponent<PassiveButton>();
                button.OnClick.AddListener((UnityAction) OnClose);
                button.OnMouseOver = new UnityEvent();
                button.OnMouseOut = new UnityEvent();
            }

            //Configure title title
            textRenderer = controlText.gameObject.GetComponent<TextMeshPro>();
            textRenderer.autoSizeTextContainer = true;
            textRenderer.horizontalAlignment = HorizontalAlignmentOptions.Center;
            controlText.transform.localPosition = new Vector3(0f, size.y / 2 * 0.85f);

            holder.gameObject.SetActive(false);
        }

        protected void SetButtonListener(Action listener) {
            if (closeButton != null) {
                PassiveButton button = closeButton.gameObject.GetComponents<PassiveButton>().Last();
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener(listener);
            }
        }

        public void OnClose() {
            holder.SetActive(false);
            IsActive = false;
        }

        public void Show() {
            holder.SetActive(true);
            IsActive = true;
        }

        public static void HudUpdate() {
            Windows.RemoveAll(item => item.holder == null || item.backgroundComponent == null || item.controlText == null);

            foreach (var window in Windows)
                window.Update();
        }

        private void Update() {
            textRenderer.text = title;
        }
    }
}
