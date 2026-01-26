using UnityEngine;

namespace Shared.Utilities
{
    public static class UIAnchorUtils
    {
        public enum AnchorType
        {
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,
            MiddleLeft = 3,
            MiddleCenter = 4,
            MiddleRight = 5,
            BottomLeft = 6,
            BottomCenter = 7,
            BottomRight = 8,
            Stretch,
            StretchHorizontally,
            StretchVertically,
            Custom
        }

        public static AnchorType GetAnchorType(RectTransform rectTransform)
        {
            if (rectTransform == null)
                return AnchorType.Custom;

            Vector2 anchorMin = rectTransform.anchorMin;
            Vector2 anchorMax = rectTransform.anchorMax;

            // Check for common anchor presets
            if (anchorMin == Vector2.zero && anchorMax == Vector2.zero)
                return AnchorType.BottomLeft;
            if (anchorMin == new Vector2(0.5f, 0) && anchorMax == new Vector2(0.5f, 0))
                return AnchorType.BottomCenter;
            if (anchorMin == new Vector2(1, 0) && anchorMax == new Vector2(1, 0))
                return AnchorType.BottomRight;

            if (anchorMin == new Vector2(0, 0.5f) && anchorMax == new Vector2(0, 0.5f))
                return AnchorType.MiddleLeft;
            if (anchorMin == new Vector2(0.5f, 0.5f) && anchorMax == new Vector2(0.5f, 0.5f))
                return AnchorType.MiddleCenter;
            if (anchorMin == new Vector2(1, 0.5f) && anchorMax == new Vector2(1, 0.5f))
                return AnchorType.MiddleRight;

            if (anchorMin == new Vector2(0, 1) && anchorMax == new Vector2(0, 1))
                return AnchorType.TopLeft;
            if (anchorMin == new Vector2(0.5f, 1) && anchorMax == new Vector2(0.5f, 1))
                return AnchorType.TopCenter;
            if (anchorMin == new Vector2(1, 1) && anchorMax == new Vector2(1, 1))
                return AnchorType.TopRight;

            if (anchorMin == Vector2.zero && anchorMax == Vector2.one)
                return AnchorType.Stretch;

            if (anchorMin.x == 0 && anchorMax.x == 1 && anchorMin.y == 0.5f && anchorMax.y == 0.5f)
                return AnchorType.StretchHorizontally;
            if (anchorMin.x == 0.5f && anchorMax.x == 0.5f && anchorMin.y == 0 && anchorMax.y == 1)
                return AnchorType.StretchVertically;

            return AnchorType.Custom;
        }
    }
}