using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using UnhollowerBaseLib;

namespace HardelAPI.Utility.Helper {
    public class ColorHelper {
        public struct CustomColor {
            public string name;
            public Color32 color;
            public Color32 shadow;
            public bool isLighterColor;
        }

        protected static Dictionary<int, string> ColorStrings = new Dictionary<int, string>();
        public static List<int> lighterColors = new List<int>() { 3, 4, 5, 7, 10, 11 };
        public static uint pickableColors = (uint) Palette.ColorNames.Length;
        public static List<CustomColor> colors = new List<CustomColor>();
        private static int LastId = 50000;

        public static void AddColor(CustomColor color) {
            List<StringNames> longlist = Enumerable.ToList<StringNames>(Palette.ColorNames);
            List<Color32> colorlist = Enumerable.ToList<Color32>(Palette.PlayerColors);
            List<Color32> shadowlist = Enumerable.ToList<Color32>(Palette.ShadowColors);

            LastId++;
            int id = LastId;
            longlist.Add((StringNames) id);
            ColorStrings[id++] = color.name;

            colorlist.Add(color.color);
            shadowlist.Add(color.shadow);

            if (color.isLighterColor)
                lighterColors.Add(colorlist.Count - 1);

            Palette.ColorNames = longlist.ToArray();
            Palette.PlayerColors = colorlist.ToArray();
            Palette.ShadowColors = shadowlist.ToArray();
            pickableColors++;
        }

        public static void Load() {
            List<StringNames> longlist = Enumerable.ToList<StringNames>(Palette.ColorNames);
            List<Color32> colorlist = Enumerable.ToList<Color32>(Palette.PlayerColors);
            List<Color32> shadowlist = Enumerable.ToList<Color32>(Palette.ShadowColors);

            int id = 50000;
            foreach (CustomColor cc in colors) {
                longlist.Add((StringNames) id);
                ColorStrings[id++] = cc.name;
                colorlist.Add(cc.color);
                shadowlist.Add(cc.shadow);
                if (cc.isLighterColor)
                    lighterColors.Add(colorlist.Count - 1);
            }

            Palette.ColorNames = longlist.ToArray();
            Palette.PlayerColors = colorlist.ToArray();
            Palette.ShadowColors = shadowlist.ToArray();
        }

        public static Color32 Shadow(Color32 color) {
            byte red = (byte) (color.r - 70);
            byte blue = (byte) (color.b - 70);
            byte green = (byte) (color.g - 70);
            byte alpha = color.a;

            red = (byte) (red < 0 ? 0 : red);
            blue = (byte) (blue < 0 ? 0 : blue);
            green = (byte) (green < 0 ? 0 : green);

            return new Color32(red, blue, green, alpha);
        }

        [HarmonyPatch]
        public static class CustomColorPatches {

            [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] {
                typeof(StringNames),
                typeof(Il2CppReferenceArray<Il2CppSystem.Object>)
            })]
            private class ColorStringPatch {
                public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name) {
                    if ((int) name >= 50000) {
                        string text = ColorStrings[(int) name];
                        if (text != null) {
                            __result = text;
                            return false;
                        }
                    }
                    return true;
                }
            }

            [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
            private static class PlayerTabEnablePatch {

                private static GameObject Inner = null;
                private static Scroller Scroll;

                private static GameObject CreateScoller(PlayerTab __instance) {
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
                    mask.sprite = SpriteHelper.LoadSpriteFromEmbeddedResources("HardelAPI.Resources.Background.png", 100f);

                    BoxCollider2D collider = SpriteMask.AddComponent<BoxCollider2D>();
                    collider.size = Vector2.one;
                    collider.enabled = true;

                    SpriteMask.SetActive(true);
                    return Inner;
                }

                public static void Postfix(PlayerTab __instance) {
                    if (Inner == null)
                        CreateScoller(__instance);

                    Il2CppArrayBase<ColorChip> chips = __instance.ColorChips.ToArray();
                    int cols = 4;
                    for (int i = 0; i < chips.Length; i++) {
                        ColorChip chip = chips[i];
                        int row = i / cols, col = i % cols;
                        chip.transform.localPosition = new Vector3(-0.9f + (col * 0.6f), 1.550f - (row * 0.55f), chip.transform.localPosition.z);
                        chip.transform.localScale *= 0.9f;
                        chip.transform.SetParent(Inner.transform);

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

                        if (i >= pickableColors)
                            chip.transform.localScale *= 0f;
                    }

                    int scrollRow = Mathf.Max((__instance.ColorChips.Count / cols) - 6, 0);
                    float YRange = (scrollRow * 0.55f) + 0.25f;
                    Scroll.YBounds = new FloatRange(0f, YRange);
                }
            }

            [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadPlayerPrefs))]
            private static class LoadPlayerPrefsPatch {
                private static bool needsPatch = false;
                public static void Prefix([HarmonyArgument(0)] bool overrideLoad) {
                    if (!SaveManager.loaded || overrideLoad)
                        needsPatch = true;
                }
                public static void Postfix() {
                    if (!needsPatch)
                        return;
                    SaveManager.colorConfig %= pickableColors;
                    needsPatch = false;
                }
            }

            [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
            private static class PlayerControlCheckColorPatch {
                private static bool isTaken(PlayerControl player, uint color) {
                    foreach (GameData.PlayerInfo p in GameData.Instance.AllPlayers)
                        if (!p.Disconnected && p.PlayerId != player.PlayerId && p.ColorId == color)
                            return true;
                    return false;
                }

                public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor) {
                    uint color = (uint) bodyColor;
                    if (isTaken(__instance, color) || color >= Palette.PlayerColors.Length) {
                        int num = 0;
                        while (num++ < 50 && (color >= pickableColors || isTaken(__instance, color))) {
                            color = (color + 1) % pickableColors;
                        }
                    }
                    __instance.RpcSetColor((byte) color);
                    return false;
                }
            }
        }
    }
}