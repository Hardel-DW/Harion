using PowerTools;
using UnityEngine;

namespace Harion.Utility.Utils {
    public static class SpriteAnimUtils {

        public static void StartAnimation(AnimationClip clip, Vector3 position, float scale, float speed = 1f) {
            GameObject gameObject = new GameObject();
            gameObject.transform.position = position;
            gameObject.transform.localScale *= scale;
            gameObject.AddComponent<SpriteRenderer>();
            gameObject.AddComponent<SpriteAnim>().Play(clip, speed);
        }
    }
}
