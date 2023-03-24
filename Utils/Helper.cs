using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.CodeDom.Compiler;

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
        
        private static List<string> keywords = new List<string>{ "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };
        
        public static bool IsValidVariableName(string name)
        {
            // Check if the name is null or empty
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            // Check if the name starts with a letter or underscore
            if (!char.IsLetter(name[0]) && name[0] != '_')
            {
                return false;
            }

            // Check if each subsequent character is a letter, digit, or underscore
            for (int i = 1; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
                {
                    return false;
                }
            }

            // Check if the name is a C# keyword
            
            if (keywords.Contains(name))
            {
                return false;
            }

            // If all checks pass, the name is valid
            return true;
        }
    }
}