using System;
using System.Collections;
using UnityEngine;

namespace HardelAPI.Utility {
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
    }
}
