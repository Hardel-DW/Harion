using Harion.Utility.Helper;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace Harion.ColorDesigner.Patch {

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
    internal static class PlayerTabPatch {

        private static GameObject Inner = null;
        private static Scroller Scroll;
        private static int Cols = 4;

        public static void Postfix(PlayerTab __instance) {
            if (Inner == null || !Inner.scene.IsValid())
                CreateScoller(__instance);

            Il2CppArrayBase<ColorChip> chips = __instance.ColorChips.ToArray();
            for (int i = 0; i < chips.Length; i++) {
                ColorChip chip = chips[i];
                int row = i / Cols, col = i % Cols;
                chip.transform.localPosition = new Vector3(-0.9f + (col * 0.6f), 1.550f - (row * 0.55f), chip.transform.localPosition.z);
                chip.transform.localScale *= 0.9f;
                chip.transform.SetParent(Inner.transform);
                UpdateChip(chip);

                if (i >= ColorCreator.pickableColors)
                    chip.transform.localScale *= 0f;
            }

            UpdateScroll(__instance);
        }

        private static void CreateScoller(PlayerTab __instance) {
            // Inner
            Inner = new GameObject { layer = 5, name = "Inner" };
            Inner.transform.SetParent(__instance.ColorTabArea);
            Inner.transform.localPosition = Vector3.zero;

            // Scroller
            GameObject Scroller = new GameObject { layer = 5, name = "Scroller" };
            Scroller.transform.SetParent(__instance.transform);

            Scroll = Scroller.AddComponent<Scroller>();
            Scroll.allowX = false;
            Scroll.allowY = true;
            Scroll.velocity = new Vector2(0.008f, 0.005f);
            Scroll.ScrollerYRange = new FloatRange(0f, 0f);
            Scroll.YBounds = new FloatRange(0f, 3f);
            Scroll.Inner = Inner.transform;

            // Mask
            GameObject SpriteMask = new GameObject();
            SpriteMask.name = "Mask";
            SpriteMask.layer = 5;
            SpriteMask.transform.SetParent(__instance.ColorTabArea);
            SpriteMask.transform.localPosition = new Vector3(0f, 0f, 0f);
            SpriteMask.transform.localScale = new Vector3(250f, 400f, 1f);

            SpriteMask mask = SpriteMask.AddComponent<SpriteMask>();
            mask.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("Harion.Resources.Background.png", 100f);

            BoxCollider2D collider = SpriteMask.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.enabled = true;

            SpriteMask.SetActive(true);
        }

        private static void UpdateChip(ColorChip chip) {
            SpriteRenderer renderer = chip.GetComponent<SpriteRenderer>();
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            GameObject ForeGround = chip.transform.GetChild(0).gameObject;
            GameObject ControllerHighlight = chip.transform.GetChild(1).gameObject;

            GameObject Shade = ForeGround.transform.GetChild(0).gameObject;
            GameObject Shade1 = ForeGround.transform.GetChild(1).gameObject;

            SpriteRenderer HighlightRenderer = ControllerHighlight.GetComponent<SpriteRenderer>();
            HighlightRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            SpriteRenderer shadeRenderer = Shade1.GetComponent<SpriteRenderer>();
            shadeRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            shadeRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Object.Destroy(ForeGround.GetComponent<SpriteMask>());
            Object.Destroy(Shade);
        }

        private static void UpdateScroll(PlayerTab __instance) {
            int scrollRow = Mathf.Max((__instance.ColorChips.Count / Cols) - 6, 0);
            float YRange = (scrollRow * 0.55f) + 0.25f;
            Scroll.YBounds = new FloatRange(0f, YRange);
        }
    }
}
