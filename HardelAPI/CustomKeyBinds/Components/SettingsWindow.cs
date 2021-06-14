using HardelAPI.Utility.Helper;
using HardelAPI.Utility.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HardelAPI.CustomKeyBinds.Components {
    public class SettingsWindow : PopUpWindow {
        public static Sprite NavigationArrow = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Navigation.png", 100f).DontDestroy();
        private static int IndexSection = 0;
        private static int StartAt = 0;
        public List<KeySelector> content = new List<KeySelector>();
        public GameObject separator;
        private OptionsMenuBehaviour optionsMenu;
        private GameObject LeftArrow;
        private GameObject RightArrow;

        public SettingsWindow(OptionsMenuBehaviour optionsMenu) : base(optionsMenu, "KeyBindPopUp", "Keybinds", new Vector2(5.5f, 6f), new Vector2(-3f, 2f)) {
            this.optionsMenu = optionsMenu;
            SetButtonListener(OnClose);
            Start(optionsMenu);
        }

        private void Start(OptionsMenuBehaviour optionsMenu) {
            separator = new GameObject("Separator");
            separator.transform.parent = holder.transform;

            Vector3 start = new Vector3(0f, size.y / 2 * 0.75f, holder.transform.position.z - 21f);
            separator.transform.position = start;
            ShowKeySelector();

            // Create LeftArrow Navigation
            LeftArrow = new GameObject();
            LeftArrow.transform.parent = separator.transform.parent;
            LeftArrow.transform.localPosition = new Vector3(-1.750f, 2.550f, 0f);
            LeftArrow.transform.localScale = new Vector3(1.5f, 0.5f, 1f);
            LeftArrow.name = "Left Arrow";
            LeftArrow.layer = 5;
            LeftArrow.AddComponent<SpriteRenderer>().sprite = NavigationArrow;
            CircleCollider2D colliderLeft = LeftArrow.AddComponent<CircleCollider2D>();
            colliderLeft.radius = 0.4f;
            colliderLeft.isTrigger = true;
            colliderLeft.offset = Vector2.zero;

            PassiveButton ButtonPassiveLeft = LeftArrow.AddComponent<PassiveButton>();
            ButtonPassiveLeft.OnClick.RemoveAllListeners();
            ButtonPassiveLeft.OnClick.AddListener((UnityAction) LeftPage);
            ButtonPassiveLeft.OnMouseOver = new UnityEvent();
            ButtonPassiveLeft.OnMouseOut = new UnityEvent();

            // Create RightArrow Navigation
            RightArrow = new GameObject();
            RightArrow.transform.parent = separator.transform.parent;
            RightArrow.transform.Rotate(new Vector3(0f, 180f, 0f));
            RightArrow.transform.localPosition = new Vector3(1.750f, 2.550f, 0f);
            RightArrow.transform.localScale = new Vector3(1.5f, 0.5f, 1f);
            RightArrow.name = "Right Arrow";
            RightArrow.layer = 5;
            RightArrow.AddComponent<SpriteRenderer>().sprite = NavigationArrow;
            CircleCollider2D colliderRight = RightArrow.AddComponent<CircleCollider2D>();
            colliderRight.radius = 0.4f;
            colliderRight.isTrigger = true;
            colliderRight.offset = Vector2.zero;

            PassiveButton ButtonPassiveRight = RightArrow.AddComponent<PassiveButton>();
            ButtonPassiveRight.OnClick.RemoveAllListeners();
            ButtonPassiveRight.OnClick.AddListener((UnityAction) RightPage);
            ButtonPassiveRight.OnMouseOver = new UnityEvent();
            ButtonPassiveRight.OnMouseOut = new UnityEvent();

            // Change Sprite to Back sprite.
            if (holder.transform.Find("CloseButton(Clone)") != null) {
                SpriteRenderer renderer = closeButton.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Back.png", 100f);
            }
        }

        public void ShowKeySelector(KeyPage action = KeyPage.Default, MovePage move = MovePage.Default) {
            // Define Index
            switch (action) {
                case KeyPage.TurnLeft:
                    IndexSection--;
                    if (IndexSection < 0)
                        IndexSection = CustomKeyBind.KeyBinds.Count - 1;
                    break;
                case KeyPage.TurnRight:
                    IndexSection++;
                    if (IndexSection >= CustomKeyBind.KeyBinds.Count)
                        IndexSection = 0;
                    break;
                case KeyPage.Default:
                    IndexSection = CustomKeyBind.KeyBinds.Count - 1;
                    break;
            }

            // Define New KeySelector
            List<CustomKeyBind> KeyBinds = CustomKeyBind.KeyBinds.ElementAt(IndexSection).Value;
            const int numRows = 7;
            const int numCollunms = 2;
            const float topPercent = 0.70f;
            const float botPercent = 0.90f;

            // Define Move
            switch (move) {
                case MovePage.MoveDown:
                    StartAt += 2;
                    if (StartAt > KeyBinds.Count - (numRows * numCollunms) + KeyBinds.Count % numCollunms) {
                        StartAt = KeyBinds.Count - (numRows * numCollunms) + KeyBinds.Count % numCollunms;
                        return;
                    }
                    break;
                case MovePage.MoveUp:
                    StartAt -= 2;
                    if (StartAt < 0) {
                        StartAt = 0;
                        return;
                    }
                    break;
                case MovePage.Default:
                    StartAt = 0;
                    break;
            }

            // Delete older KeySelector
            foreach (KeySelector KeySelector in content)
                Object.Destroy(KeySelector.Holder);
            content = new List<KeySelector>();

            // Placement
            int IndexPosition = 0;
            for (int i = StartAt; i < (numRows * numCollunms) + StartAt; i++) {
                float x = IndexPosition % numCollunms;
                float y = (IndexPosition - x) / numCollunms;
                IndexPosition++;

                if (KeyBinds.ElementAtOrDefault(i) == null)
                    continue;

                CustomKeyBind KeyBind = KeyBinds[i];
                Vector2 selectorPos = new Vector2(-size.x / 2 * botPercent + x * (size.x / 2), size.y / 2 * topPercent - y * (size.y * (topPercent + botPercent) / 2 / numRows));
                content.Add(new KeySelector(
                    optionsMenu,
                    KeyBind.Key.ToString(),
                    KeyBind.Name,
                    KeyBind,
                    new Vector2(selectorPos.x + 0.85f, selectorPos.y - 0.2f),
                    selectorPos + new Vector2(size.x / 3, 0f),
                    holder)
                );
            }

            // Title
            title = CustomKeyBind.KeyBinds.ElementAt(IndexSection).Key;
        }

        public new void OnClose() {
            foreach (var selector in content)
                selector.OnClose();

            base.OnClose();
        }

        private void LeftPage() => ShowKeySelector(KeyPage.TurnLeft);

        private void RightPage() => ShowKeySelector(KeyPage.TurnRight);
    }
}
