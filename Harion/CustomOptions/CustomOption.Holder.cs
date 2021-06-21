using Harion.Reactor;
using Harion.Utility.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Harion.CustomOptions {
    /// <summary>
    /// A derivative of <see cref="CustomOptionButton"/>, handling option headers.
    /// </summary>
    public class CustomOptionHolder : CustomOptionButton {

        private SpriteRenderer ArrowRenderer;
        private GameObject HolderArrow;

        /// <summary>
        /// Adds an option header.
        /// </summary>
        /// <param name="title">The title of the header</param>
        /// <param name="menu">The header will be visible in the lobby options menu</param>
        /// <param name="hud">The header will appear in the HUD (option list) in the lobby</param>
        /// <param name="initialValue">The header's initial (client sided) value, can be used to hide/show other options</param>
        public CustomOptionHolder(string title, bool menu = true, bool hud = true, bool initialValue = false, CustomOption parent = null) : base(title, menu, hud, initialValue, parent) { }

        protected override bool GameObjectCreated(OptionBehaviour o) {
            GameObject CheckBox = base.GameObject.transform.FindChild("CheckBox")?.gameObject;
            CheckBox.SetActive(false);
            
            if (HolderArrow == null || !HolderArrow.scene.IsValid()) {
                HolderArrow = new GameObject { name = "HolderArrow", layer = 5 };
                HolderArrow.transform.SetParent(CheckBox.transform.parent);
                HolderArrow.transform.localPosition = new Vector3(1.525f, 0f, -1f);
                HolderArrow.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

                ArrowRenderer = HolderArrow.AddComponent<SpriteRenderer>();
                ArrowRenderer.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("Harion.Resources.ArrowOption.png", 100f);
                ArrowRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                PassiveButton Passive = HolderArrow.AddComponent<PassiveButton>();
                Passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                Passive.OnMouseOver = new UnityEvent();
                Passive.OnMouseOver.AddListener((UnityAction) OnMouseOver);
                Passive.OnMouseOut = new UnityEvent();
                Passive.OnMouseOut.AddListener((UnityAction) OnMouseOut);
            }
            
            return UpdateGameObject();
        }

        private void OnMouseOver() {
            ArrowRenderer.SetOutline(new Color(1f, 1f, 0f, 1f));
            ArrowRenderer.color = new Color(1f, 1f, 1f, 1f);
        }

        private void OnMouseOut() {
            ArrowRenderer.SetOutline(new Color(1f, 1f, 0f, 0f));
            ArrowRenderer.color = new Color(1f, 1f, 1f, 0.75f);
        }

        /// <summary>
        /// Toggles the option value (called when the header is pressed).
        /// </summary>
        public override void Toggle() {
            base.Toggle();
            ShowChildren(GetValue(), false, true);
            Coroutines.Start(SwapArrow(GetValue()));
        }

        private IEnumerator SwapArrow(bool ToUp) {
            if (ArrowRenderer == null)
                yield return true;

            float RotateAttemp = ToUp ? 180f : 0f;
            Quaternion GotoPosition = Quaternion.Euler(ArrowRenderer.transform.rotation.x, ArrowRenderer.transform.rotation.y, RotateAttemp);
            float elapsedTime = 0f;
            float waitTime = 0.5f;

            while (elapsedTime < waitTime) {
                ArrowRenderer.transform.rotation = Quaternion.Lerp(ArrowRenderer.transform.rotation, GotoPosition, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ArrowRenderer.transform.rotation = GotoPosition;
            yield return null;
        }
    }

    public partial class CustomOption {
        /// <summary>
        /// Adds an option header.
        /// </summary>
        /// <param name="title">The title of the header</param>
        /// <param name="menu">The header will be visible in the lobby options menu</param>
        /// <param name="hud">The header will appear in the HUD (option list) in the lobby</param>
        /// <param name="initialValue">The header's initial (client sided) value, can be used to hide/show other options</param>
        public static CustomOptionHolder AddHolder(string title, bool menu = true, bool hud = true, bool initialValue = false, CustomOption parent = null) {
            return new CustomOptionHolder(title, menu, hud, initialValue, parent);
        }
    }
}