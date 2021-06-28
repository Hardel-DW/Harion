using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Harion.Utility.Utils {
    public static class GameObjectUtils {

        /// <summary>
        /// Change the player size smoothly.
        /// </summary>
        /// <param name="Player">Player who change scale</param>
        /// <param name="Duration">The duration in float (Seconds)</param>
        /// <param name="Size">Size, the new size of player after ended effect</param>
        public static IEnumerator ChangeSize(GameObject gameObject, float Duration, float Size) {
            float elapsedTime = 0;

            while (elapsedTime < Duration) {
                gameObject.transform.localScale = Vector2.Lerp(gameObject.transform.localScale, new Vector2(Size, Size), (elapsedTime / Duration));

                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return true;
        }

        /// <summary>
        /// Change the player size smoothly.
        /// </summary>
        /// <param name="Player">Player who change scale</param>
        /// <param name="Duration">The duration in float (Seconds)</param>
        /// <param name="Size">Size, the new size of player after ended effect</param>
        /// <param name="EndedAction">Do Something when it's ended of function</param>
        public static IEnumerator ChangeSize(GameObject gameObject, float Duration, float Size, Action EndedAction) {
            float elapsedTime = 0;

            while (elapsedTime < Duration) {
                gameObject.transform.localScale = Vector2.Lerp(gameObject.transform.localScale, new Vector2(Size, Size), (elapsedTime / Duration));

                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            EndedAction();
            yield return true;
        }

        public static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration = 0.75f) {
            var temp = default(Vector3);
            temp.z = target.position.z;
            for (var time = 0f; time < duration; time += Time.deltaTime) {
                var t = time / duration;
                temp.x = Mathf.SmoothStep(source.x, dest.x, t);
                temp.y = Mathf.SmoothStep(source.y, dest.y, t);
                target.position = temp;
                yield return null;
            }

            temp.x = dest.x;
            temp.y = dest.y;
            target.position = temp;
        }

        public static T GetChildComponentByName<T>(Component parent, string name) where T : Component {
            return parent.GetComponentsInChildren<T>(true).FirstOrDefault(component => component.gameObject.name == name);
        }

        public static T GetChildComponentByName<T>(GameObject parent, string name) where T : Component {
            return parent.GetComponentsInChildren<T>(true).FirstOrDefault(component => component.gameObject.name == name);
        }

        public static bool CompareName(this GameObject a, GameObject b) => a == b || string.Equals(a?.name, b?.name, StringComparison.Ordinal);

        public static bool CompareName(this OptionBehaviour a, OptionBehaviour b) => CompareName(a?.gameObject, b?.gameObject);

        /// <summary>
        /// Stops <paramref name="obj"/> from being destroyed
        /// </summary>
        /// <param name="obj">Object to stop from being destroyed</param>
        /// <returns>Passed <paramref name="obj"/></returns>
        public static T DontDestroy<T>(this T obj) where T : UnityEngine.Object {
            obj.hideFlags |= HideFlags.HideAndDontSave;

            return obj.DontDestroyOnLoad();
        }

        public static T DontUnload<T>(this T obj) where T : UnityEngine.Object {
            obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            return obj;
        }

        public static T DontDestroyOnLoad<T>(this T obj) where T : UnityEngine.Object {
            UnityEngine.Object.DontDestroyOnLoad(obj);

            return obj;
        }

        public static void Destroy(this UnityEngine.Object obj) {
            UnityEngine.Object.Destroy(obj);
        }

        public static void DestroyImmediate(this UnityEngine.Object obj) {
            UnityEngine.Object.DestroyImmediate(obj);
        }

        public static string GameObjectPath(GameObject obj) {
            string path = "/" + obj.name;
            while (obj.transform.parent != null) {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }

            return path;
        }

        public static GameObject FindObject(this GameObject parent, string name) {
            Transform[] transforms = parent.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms) {
                if (transform.name == name) {
                    return transform.gameObject;
                }
            }

            return null;
        }

        public static GameObject FindObjectSiblings(this GameObject parent, string name) {
            Transform[] transforms = parent.transform.parent.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms) {
                if (transform.name == name) {
                    return transform.gameObject;
                }
            }

            return null;
        }

        public static void ChangeX(this ref Vector3 v, float x) {
            v = new Vector3(x, v.y, v.z);
        }   

        public static void ChangeY(this ref Vector3 v, float y) {
            v = new Vector3(v.x, y, v.z);
        }

        public static void ChangeZ(this ref Vector3 v, float z) {
            v = new Vector3(v.x, v.y, z);
        }
    }
}
