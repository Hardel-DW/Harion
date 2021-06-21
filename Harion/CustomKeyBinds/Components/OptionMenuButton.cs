using Harion.Utility.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Harion.CustomKeyBinds.Components {
    public class OptionsMenuButton {
        private static readonly List<OptionsMenuButton> Buttons = new List<OptionsMenuButton>();
        public Component backgroundComponent;
        public PassiveButton button;
        public GameObject holder;
        public TextMeshPro textComponent;
        public Action action;
        public string name;
        public Vector2 pos;
        public Vector2 size;
        public string text;

        public OptionsMenuButton(OptionsMenuBehaviour optionsMenu, string name, string text, Action action, Vector2 pos, GameObject parent = null) : this(optionsMenu, name, text, action, pos, new Vector2(2.0f, 0.4f), parent) { }

        public OptionsMenuButton(OptionsMenuBehaviour optionsMenu, string name, string text, Action action, Vector2 pos, Vector2 size, GameObject parent = null) {
            this.text = text;
            this.pos = pos;
            this.name = name;
            this.action = action;
            this.size = size;

            Buttons.Add(this);
            Start(optionsMenu, parent);
        }

        private void Start(OptionsMenuBehaviour optionsMenu, GameObject parent) {
            //Get original components
            Component joyStickButtonComponent = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "JoystickModeButton");
            Component oBackgroundComponent = GameObjectUtils.GetChildComponentByName<Component>(joyStickButtonComponent, "Background");
            TextMeshPro oTextComponent = GameObjectUtils.GetChildComponentByName<TextMeshPro>(joyStickButtonComponent, "Text_TMP");

            //Create component and add it to the parent
            holder = new GameObject(name);

            //Set button parent
            holder.transform.parent = parent == null ? joyStickButtonComponent.transform.parent.transform : parent.transform;

            //Configure holder and his components
            holder.layer = 5;
            holder.transform.localScale = new Vector3(1f, 1f, 1f);
            BoxCollider2D collider = holder.gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;

            //clone original sub components
            backgroundComponent = Object.Instantiate(oBackgroundComponent, holder.transform);
            textComponent = Object.Instantiate(oTextComponent, holder.transform);

            if (backgroundComponent == null)
                HarionPlugin.Logger.LogError($"backgroundComponent in OptionsMenuButton does not exist !");


            if (textComponent == null)
                HarionPlugin.Logger.LogError($"textComponent in OptionsMenuButton does not exist !");

            //Configure background size
            SpriteRenderer renderer = backgroundComponent.gameObject.GetComponent<SpriteRenderer>();
            renderer.size = size;

            //Add PassiveButton component 
            button = holder.gameObject.AddComponent<PassiveButton>();
            button.OnClick.AddListener(action);
            button.OnMouseOut = new UnityEvent();
            button.OnMouseOut.AddListener((UnityAction) OnOut);
            button.OnMouseOver = new UnityEvent();
            button.OnMouseOver.AddListener((UnityAction) OnOver);

            //Enable the button
            OnOut();
            holder.gameObject.SetActive(true);
            textComponent.text = text;
            holder.transform.localPosition = new Vector3(pos.x, pos.y);
        }

        public void SetButtonBgColor(Color color) {
            var renderer = backgroundComponent.gameObject.GetComponent<SpriteRenderer>();
            renderer.color = color;
        }

        internal void SetLabel(string label) {
            textComponent.text = label;
        }

        public void OnOut() {
            SetButtonBgColor(new Color(1f, 1f, 1f, 1f));
        }

        public void OnOver() {
            SetButtonBgColor(new Color(0f, 1f, 0.1647059f, 1f));
        }

        internal void SetOnMouseOver(Action onMouseOver) {
            button.OnMouseOver.RemoveAllListeners();
            button.OnMouseOver.AddListener(onMouseOver);
        }

        internal void SetOnMouseOut(Action onMouseOut) {
            button.OnMouseOut.RemoveAllListeners();
            button.OnMouseOut.AddListener(onMouseOut);
        }

        internal void SetOnClick(Action onCLick) {
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener(onCLick);
        }

        public static void HudUpdate() {
            Buttons.RemoveAll(item =>
                item.holder == null || item.textComponent == null || item.backgroundComponent == null ||
                item.button == null);
            foreach (var button in Buttons)
                button.Update();
        }

        private void Update() { }
    }
}