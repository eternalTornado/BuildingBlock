using UnityEngine;

namespace CodeStringers.Framework {
    public class SafeArea : MonoBehaviour {
        //private RectTransform _rectTransform;
        //private Rect _safeArea;
        //private Vector2 _minAnchor;
        //private Vector2 _maxAnchor;

        //private void Awake() {
        //    _rectTransform = GetComponent<RectTransform>();
        //    _safeArea = Screen.safeArea;
        //    _minAnchor = _safeArea.position;
        //    _maxAnchor = _minAnchor + _safeArea.size;

        //    _minAnchor.x /= Screen.width;
        //    _minAnchor.y /= Screen.height;
        //    _maxAnchor.x /= Screen.width;
        //    _maxAnchor.y /= Screen.height;

        //    _rectTransform.anchorMin = _minAnchor;
        //    _rectTransform.anchorMax = _maxAnchor;
        //}

        RectTransform Panel;
        Rect LastSafeArea = new Rect(0, 0, 0, 0);

        void Awake()
        {
            Panel = GetComponent<RectTransform>();
            Refresh();
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
            return Screen.safeArea;
        }

        void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            Panel.anchorMin = anchorMin;
            Panel.anchorMax = anchorMax;

            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}

