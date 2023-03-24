using UnityEngine;

namespace Maniac.Utils.Extension
{
    public static class TransformExtension
    {
        public static Transform ClearAllChildren(this Transform transform)
        {
            if (transform.childCount == 0) return transform;

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }

        public static void CopyDataFrom(this Transform destination, Transform source)
        {
            destination.SetPositionAndRotation(source.position,source.rotation);
            destination.localScale = source.localScale;
        }
    }
}