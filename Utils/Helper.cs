using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maniac.Utils
{
    public static class Helper
    {
        private static Camera camera;
        public static Camera Camera
        {
            get
            {
                if (camera == null) camera = Camera.main;
                return camera;
            }
        }

        private static PointerEventData eventDataCurrentPosition;
        private static List<RaycastResult> results;
        
        private static readonly Dictionary<float, WaitForSeconds> _waitForSecondsMap = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if(!_waitForSecondsMap.ContainsKey(seconds))
                _waitForSecondsMap.Add(seconds,new WaitForSeconds(seconds));

            return _waitForSecondsMap[seconds];
        }
        
        
        public static bool IsOverUI()
        {
            eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform rect)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, rect.position, Camera, out Vector3 result);
            return result;
        }

        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t)
            {
                Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// percent is in 0 -> 1, 0 means always false, vise versa
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static bool IsPercentTrigger(float percent)
        {
            return Random.Range(0f, 1f) <= percent;
        }
        
        public static string ToRGBHex(Color c)
        {
            return $"#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}";
        }
 
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
    }
}