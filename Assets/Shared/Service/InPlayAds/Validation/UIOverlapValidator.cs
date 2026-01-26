using System.Linq;
using Shared.Utilities.SharedBehaviour;
using UnityEngine;

namespace Shared.Service.InPlayAds.Validation
{
    [DisallowMultipleComponent]
    public class UIOverlapValidator : SharedMonoBehaviour
    {
        [SerializeField] private RectTransform placement;
        [SerializeField] private RectTransform[] otherRects;

        public bool Validate() => otherRects.All(r => !AreRectTransformsOverlapping(placement, r));

        private static bool AreRectTransformsOverlapping(RectTransform rect1, RectTransform rect2)
        {
            if (!rect2.gameObject.activeInHierarchy) return false;
            // Get world-space rectangles
            var rect1World = GetWorldRect(rect1);
            var rect2World = GetWorldRect(rect2);

            // Check if the rectangles overlap
            return rect1World.Overlaps(rect2World);
        }

        private static Rect GetWorldRect(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // Bottom left corner (min) and top right corner (max)
            Vector2 min = corners[0];
            Vector2 max = corners[2];

            return new Rect(min, max - min);
        }
    }
}