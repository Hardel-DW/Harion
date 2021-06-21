using Harion.Utility.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Harion.CustomKeyBinds.Components {
    public class KeySelector {
        private static readonly List<KeySelector> Selectors = new List<KeySelector>();
        private static readonly KeyCode[] KeyCodes = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
        private static KeySelector IsSelecting;
        private static Component IgnoreClose;
        public static bool CanEscape = true;

        private OptionsMenuButton Button;
        private TextMeshPro Label;
        //private TextMeshPro TextMeshPro;
        public GameObject Holder;

        public CustomKeyBind key;
        public Vector2 labelPos;
        public Vector2 buttonPos;
        public string label;
        public string name;
        public bool IsDoubleKey;

        //Setup key listener
        public KeySelector(OptionsMenuBehaviour optionsMenu, string name, string label, CustomKeyBind key, Vector2 labelPos, Vector2 buttonPos, GameObject parent = null) {
            this.name = name;
            this.label = label;
            this.key = key;
            this.labelPos = labelPos;
            this.buttonPos = buttonPos;
            IsDoubleKey = false;

            Selectors.Add(this);
            Start(optionsMenu, parent);
        }

        private void Start(OptionsMenuBehaviour optionsMenu, GameObject parent) {
            //Get original components
            Component oControlGroup = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "ControlGroup");
            Component oIgnoreClose = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "IgnoreClose");
            Component joyStickButtonComponent = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "JoystickModeButton");
            TextMeshPro oTextComponent = GameObjectUtils.GetChildComponentByName<TextMeshPro>(joyStickButtonComponent, "Text_TMP");

            if (oIgnoreClose == null)
                oIgnoreClose = GameObjectUtils.GetChildComponentByName<Component>(optionsMenu, "CloseBackground");

            //Create component and add it to the parent
            Holder = new GameObject(name);

            //Set button parent
            Holder.transform.parent = parent == null ? optionsMenu.transform : parent.transform;

            //Configure holder and his components
            Holder.layer = 5; //UI
            Holder.transform.localScale = new Vector3(1f, 1f, 1f);
            Holder.transform.localPosition = Vector3.zero;

            //clone original sub components
            Label = Object.Instantiate(oTextComponent, Holder.transform);
            IgnoreClose = Object.Instantiate(oIgnoreClose, Holder.transform);
            if (Label == null)
                HarionPlugin.Logger.LogError($"Label in KeySelector does not exist !");

            if (IgnoreClose == null)
                HarionPlugin.Logger.LogError($"IgnoreClose in KeySelector does not exist !");


            //Configure label
            //TextMeshPro = Label.gameObject.GetComponent<TextMeshPro>();
            Label.transform.localPosition = new Vector3(labelPos.x, labelPos.y);
            Label.text = label;
            Label.horizontalAlignment = HorizontalAlignmentOptions.Left;
            Label.fontStyle = FontStyles.Bold;

            //Create button
            Button = new OptionsMenuButton(optionsMenu, key + "Button", key.Key.ToString(), OnClick, buttonPos + new Vector2(0f, -0.2f), new Vector2(1.0f, 0.4f), Holder);
            Button.SetOnMouseOut(OnMouseOut);
            Button.SetOnMouseOver(OnMouseOver);

            //Configure ignoreClose
            IgnoreClose.gameObject.SetActive(false);
            IgnoreClose.transform.localPosition = new Vector3(IgnoreClose.transform.localPosition.x,
                IgnoreClose.transform.localPosition.y, IgnoreClose.transform.localPosition.z - 1000f);

            BoxCollider2D collider = IgnoreClose.gameObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(Screen.width, Screen.height);
        }

        private static void OnInput() {
            if (IsSelecting == null)
                return;

            foreach (KeyCode code in KeyCodes) {
                if (!Input.GetKey(code))
                    continue;

                IsSelecting?.key.SetKey(code);
                IsSelecting?.Button.SetLabel(code.ToString());
                var backup = IsSelecting;
                IsSelecting = null;
                IgnoreClose.gameObject.SetActive(false);
                backup?.OnMouseOut();

                if (code == KeyCode.Escape)
                    CanEscape = false;
            }
        }

        private void OnClick() {
            if (IsSelecting != null)
                return;

            IgnoreClose.gameObject.SetActive(true);
            IsSelecting = this;
            Button.SetButtonBgColor(new Color(1f, 0.8f, 0f, 1f));
            Button.SetLabel("...");
        }

        private void OnMouseOut() {
            if (IsSelecting != null)
                return;

            Button.OnOut();
        }

        private void OnMouseOver() {
            if (IsSelecting != null)
                return;

            Button.OnOver();
        }

        public void OnClose() {
            IsSelecting = null;
            IgnoreClose.gameObject.SetActive(false);
            OnMouseOut();

            Button.SetLabel(key.Name);
        }

        public static void HudUpdate() {
            Selectors.RemoveAll(item => item.Holder == null);
            if (IsSelecting != null && Input.anyKeyDown)
                OnInput();

            foreach (var selector in Selectors)
                selector.Update();
        }

        private void Update() {
            Label.text = label;
        }
    }
}
