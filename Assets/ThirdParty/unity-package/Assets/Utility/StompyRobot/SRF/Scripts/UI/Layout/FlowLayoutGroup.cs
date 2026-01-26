namespace SRF.UI.Layout
{
    using System.Collections.Generic;
    using Internal;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Layout Group controller that arranges children in rows and columns, fitting elements based on specified row and column counts.
    /// </summary>
    [AddComponentMenu(ComponentMenuPaths.FlowLayoutGroup)]
    public class FlowLayoutGroup : LayoutGroup
    {
        private readonly IList<RectTransform> _rowList = new List<RectTransform>();

        private float _layoutHeight;
        public bool ChildForceExpandHeight = false;
        public bool ChildForceExpandWidth = false;
        public float Spacing = 0f;

        // New properties for controlling rows and columns
        public int MaxRows = 0; // 0 means no limit
        public int MaxColumns = 0; // 0 means no limit

        protected bool IsCenterAlign => childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.MiddleCenter ||
                                         childAlignment == TextAnchor.UpperCenter;

        protected bool IsRightAlign => childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.MiddleRight ||
                                        childAlignment == TextAnchor.UpperRight;

        protected bool IsMiddleAlign => childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleRight ||
                                         childAlignment == TextAnchor.MiddleCenter;

        protected bool IsLowerAlign => childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerRight ||
                                        childAlignment == TextAnchor.LowerCenter;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            var minWidth = GetGreatestMinimumChildWidth() + padding.left + padding.right;

            SetLayoutInputForAxis(minWidth, -1, -1, 0);
        }

        public override void SetLayoutHorizontal()
        {
            SetLayout(rectTransform.rect.width, 0, false);
        }

        public override void SetLayoutVertical()
        {
            SetLayout(rectTransform.rect.width, 1, false);
        }

        public override void CalculateLayoutInputVertical()
        {
            _layoutHeight = SetLayout(rectTransform.rect.width, 1, true);
        }

        public float SetLayout(float width, int axis, bool layoutInput)
        {
            var groupHeight = rectTransform.rect.height;
            var workingWidth = rectTransform.rect.width - padding.left - padding.right;
            var yOffset = IsLowerAlign ? padding.bottom : (float)padding.top;

            var currentRowWidth = 0f;
            var currentRowHeight = 0f;
            var currentRowCount = 0;
            var currentColumnCount = 0;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                var index = IsLowerAlign ? rectChildren.Count - 1 - i : i;
                var child = rectChildren[index];

                var childWidth = LayoutUtility.GetPreferredSize(child, 0);
                var childHeight = LayoutUtility.GetPreferredSize(child, 1);

                childWidth = Mathf.Min(childWidth, workingWidth);

                if (_rowList.Count > 0)
                {
                    currentRowWidth += Spacing;
                }

                if (MaxColumns > 0 && currentColumnCount >= MaxColumns)
                {
                    currentRowWidth -= Spacing;

                    if (!layoutInput)
                    {
                        var h = CalculateRowVerticalOffset(groupHeight, yOffset, currentRowHeight);
                        LayoutRow(_rowList, currentRowWidth, currentRowHeight, workingWidth, padding.left, h, axis);
                    }

                    _rowList.Clear();
                    yOffset += currentRowHeight + Spacing;

                    currentRowHeight = 0;
                    currentRowWidth = 0;
                    currentColumnCount = 0;
                    currentRowCount++;

                    if (MaxRows > 0 && currentRowCount >= MaxRows)
                    {
                        break;
                    }
                }

                currentRowWidth += childWidth;
                _rowList.Add(child);

                if (childHeight > currentRowHeight)
                {
                    currentRowHeight = childHeight;
                }

                currentColumnCount++;
            }

            if (!layoutInput)
            {
                var h = CalculateRowVerticalOffset(groupHeight, yOffset, currentRowHeight);
                LayoutRow(_rowList, currentRowWidth, currentRowHeight, workingWidth, padding.left, h, axis);
            }

            _rowList.Clear();

            yOffset += currentRowHeight;
            yOffset += IsLowerAlign ? padding.top : padding.bottom;

            if (layoutInput)
            {
                if (axis == 1)
                {
                    SetLayoutInputForAxis(yOffset, yOffset, -1, axis);
                }
            }

            return yOffset;
        }

        private float CalculateRowVerticalOffset(float groupHeight, float yOffset, float currentRowHeight)
        {
            float h;

            if (IsLowerAlign)
            {
                h = groupHeight - yOffset - currentRowHeight;
            }
            else if (IsMiddleAlign)
            {
                h = groupHeight * 0.5f - _layoutHeight * 0.5f + yOffset;
            }
            else
            {
                h = yOffset;
            }
            return h;
        }

        protected void LayoutRow(IList<RectTransform> contents, float rowWidth, float rowHeight, float maxWidth,
            float xOffset, float yOffset, int axis)
        {
            var xPos = xOffset;

            if (!ChildForceExpandWidth && IsCenterAlign)
            {
                xPos += (maxWidth - rowWidth) * 0.5f;
            }
            else if (!ChildForceExpandWidth && IsRightAlign)
            {
                xPos += (maxWidth - rowWidth);
            }

            var extraWidth = 0f;

            if (ChildForceExpandWidth)
            {
                var flexibleChildCount = 0;

                for (var i = 0; i < _rowList.Count; i++)
                {
                    if (LayoutUtility.GetFlexibleWidth(_rowList[i]) > 0f)
                    {
                        flexibleChildCount++;
                    }
                }

                if (flexibleChildCount > 0)
                {
                    extraWidth = (maxWidth - rowWidth) / flexibleChildCount;
                }
            }

            for (var j = 0; j < _rowList.Count; j++)
            {
                var index = IsLowerAlign ? _rowList.Count - 1 - j : j;
                var rowChild = _rowList[index];

                var rowChildWidth = LayoutUtility.GetPreferredSize(rowChild, 0);

                if (LayoutUtility.GetFlexibleWidth(rowChild) > 0f)
                {
                    rowChildWidth += extraWidth;
                }

                var rowChildHeight = LayoutUtility.GetPreferredSize(rowChild, 1);

                if (ChildForceExpandHeight)
                {
                    rowChildHeight = rowHeight;
                }

                rowChildWidth = Mathf.Min(rowChildWidth, maxWidth);

                var yPos = yOffset;

                if (IsMiddleAlign)
                {
                    yPos += (rowHeight - rowChildHeight) * 0.5f;
                }
                else if (IsLowerAlign)
                {
                    yPos += (rowHeight - rowChildHeight);
                }

                if (axis == 0)
                {
                    SetChildAlongAxis(rowChild, 0, xPos, rowChildWidth);
                }
                else
                {
                    SetChildAlongAxis(rowChild, 1, yPos, rowChildHeight);
                }

                xPos += rowChildWidth + Spacing;
            }
        }

        public float GetGreatestMinimumChildWidth()
        {
            var max = 0f;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                var w = LayoutUtility.GetMinWidth(rectChildren[i]);

                max = Mathf.Max(w, max);
            }

            return max;
        }
    }
}
