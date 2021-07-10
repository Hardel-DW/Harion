using Harion.Utility.Utils;
using System;
using System.IO;
using System.Reflection;
using UnhollowerBaseLib;
using UnityEngine;

namespace Harion.Utility.Helper {
    public static class SpriteHelper {
        private static SpriteRenderer herePoint = null;

        public static SpriteRenderer HerePoint {
            get => herePoint ??= GetHerePoint();
            set => herePoint = value;
        }

        private static SpriteRenderer GetHerePoint() {
            if (HudManager.Instance == null) {
                DestroyableSingleton<HudManager>.Instance.ShowMap((Action<MapBehaviour>) (map => {
                    map.gameObject.SetActive(false);
                    map.HerePoint.enabled = true;
                }));
            }

            return UnityEngine.Object.Instantiate(MapBehaviour.Instance.HerePoint);
        }

        public static Sprite LoadSpriteFromByte(byte[] resource, float PixelPerUnit) {
            try {
                Texture2D myTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                LoadImage(myTexture, resource, true);
                return Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
            } catch { }
            return null;
        }

        public static Sprite LoadSpriteFromEmbeddedResources(string resource, float PixelPerUnit, Assembly assembly = null) {
            try {
                Assembly myAssembly = null;
                myAssembly = assembly == null ? Assembly.GetCallingAssembly() : assembly;
                Stream myStream = Assembly.Load(myAssembly.GetName()).GetManifestResourceStream(resource);

                byte[] image = new byte[myStream.Length];
                myStream.Read(image, 0, (int) myStream.Length);
                Texture2D myTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                LoadImage(myTexture, image, true);
                return Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
            } catch { }
            return null;
        }

        public static Sprite LoadSpriteFromEmbeddedResources(string resource, Assembly assembly = null) {
            try {
                Assembly myAssembly = null;
                myAssembly = assembly == null ? Assembly.GetCallingAssembly() : assembly;
                Stream myStream = Assembly.Load(myAssembly.GetName()).GetManifestResourceStream(resource);

                byte[] image = new byte[myStream.Length];
                myStream.Read(image, 0, (int) myStream.Length);
                Texture2D myTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                LoadImage(myTexture, image, true);
                return Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), myTexture.width);
            } catch { }
            return null;
        }

        public static Sprite LoadHatSprite(string resource, Assembly assembly = null) {
            try {
                Assembly myAssembly = null;
                myAssembly = assembly == null ? Assembly.GetCallingAssembly() : assembly;
                Stream myStream = Assembly.Load(myAssembly.GetName()).GetManifestResourceStream(resource);

                byte[] image = new byte[myStream.Length];
                myStream.Read(image, 0, (int) myStream.Length);
                Texture2D myTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                LoadImage(myTexture, image, true);
                
                Sprite sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.53f, 0.575f), 100f);
                sprite.DontDestroy();
                return sprite;
            } catch { }
            return null;
        }

        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        private static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable) {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

            var il2cppArray = (Il2CppStructArray<byte>) data;

            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }

        public static void SetColorAlpha(this SpriteRenderer renderer, float alpha) {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        }
    }
}
