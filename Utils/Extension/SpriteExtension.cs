using System.Collections.Generic;
using UnityEngine;

namespace Maniac.Utils.Extension
{
    public static class SpriteExtension
    {
        /// <summary>
        ///   Returns corners of a sprite in order [TopLeft, TopRight, BottomRight, BottomLeft]
        /// </summary>
        public static List<Vector3> GetSpriteCorners(this SpriteRenderer renderer)
        {
            Vector3 topLeft = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y, 0));
            Vector3 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
            Vector3 botRight = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y, 0));
            Vector3 botLeft = renderer.transform.TransformPoint(renderer.sprite.bounds.min);
            return new List<Vector3>() { topLeft, topRight, botRight, botLeft };
        }
    }
}