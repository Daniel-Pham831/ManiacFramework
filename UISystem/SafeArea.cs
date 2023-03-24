using UnityEngine;

namespace Maniac.UISystem
{
    [ExecuteInEditMode]
    public class SafeArea : MonoBehaviour
    {
        RectTransform Panel;
        Rect LastSafeArea = new Rect(0, 0, 0, 0);

        private void Start()
        {
            Panel = GetComponent<RectTransform>();
            ApplySafeArea(GetSafeArea());
        }

        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
                ApplySafeArea(safeArea);
        }

        Rect GetSafeArea()
        {
            return UnityEngine.Screen.safeArea;
        }

        void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= UnityEngine.Screen.width;
            anchorMin.y /= UnityEngine.Screen.height;
            anchorMax.x /= UnityEngine.Screen.width;
            anchorMax.y /= UnityEngine.Screen.height;
            Panel.anchorMin = anchorMin;
            Panel.anchorMax = anchorMax;

            // Debug.LogFormat ("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            // name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}