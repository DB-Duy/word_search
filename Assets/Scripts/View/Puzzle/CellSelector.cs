// `Assets/Scripts/View/Puzzle/CellSelector.cs`

using System;
using System.Collections.Generic;
using System.Text;
using Service.Puzzle;
using Shared;
using Shared.Utils;
using UnityEngine;

namespace View.Puzzle
{
    public class CellSelector : ISharedUtility
    {
        private GamePuzzle _level;
        private readonly List<int> _selectedIndices;
        private readonly List<int> _tempIndices = new List<int>();
        private readonly PuzzleBuilder _builder;
        private bool _isSelecting = false;
        private readonly PuzzleService _puzzleService;
        private readonly PuzzleView _puzzleView;
        private readonly SelectedWordView _selectedWordView;
        private readonly SelectionLineHandler _selectionLineHandler;
        private readonly TargetWordsView _targetWordsView;

        private float _cellDetectRadiusFactor = 0.35f;
        private static StringBuilder _sb = new();

        private int _startIndex = -1;
        private int _currentCellIndex = -1;
        private Action OnPlayAnimCompleted;

        public CellSelector(PuzzleBuilder builder, PuzzleService puzzleService, PuzzleView puzzleView,
            SelectionLineHandler selectionLineHandler, SelectedWordView selectedWordView,
            TargetWordsView targetWordsView)
        {
            _builder = builder;
            _selectionLineHandler = selectionLineHandler;
            _puzzleService = puzzleService;
            _puzzleView = puzzleView;
            _selectedWordView = selectedWordView;
            _targetWordsView = targetWordsView;
            OnPlayAnimCompleted = () =>
            {
                _selectedWordView.SetWord(null);
            };
            _selectedIndices = new List<int>();
        }

        public void SetLevel(GamePuzzle level)
        {
            _level = level;
        }

        public void SetCellDetectRadiusFactor(float factor)
        {
            _cellDetectRadiusFactor = Mathf.Clamp01(factor);
        }

        public void RegisterInputHandler(PuzzleInputHandler puzzleInputHandler)
        {
            puzzleInputHandler.RegisterOnPointerDown(OnPointerDown);
            puzzleInputHandler.RegisterOnPointerUp(OnPointerUp);
            puzzleInputHandler.RegisterOnPointerMove(OnPointerMove);
        }

        private void OnPointerDown(PuzzleTouchData data)
        {
            if (_puzzleView.IsRotating) return;
            _isSelecting = false;
            _startIndex = -1;

            if (!TryGetCellIndex(data.Position, out var idx))
                return;

            _isSelecting = true;
            _startIndex = idx;

            ReplaceSelectionSingle(idx);
        }

        private void OnPointerMove(PuzzleTouchData data)
        {
            if (!_isSelecting || _startIndex < 0 || _level == null)
                return;

            if (!TryGetGridLocal(data.Position, out var local))
                return;

            if (!TryGetClosestReachableEndFromStart(_startIndex, local, out int endIdx))
                return;

            _currentCellIndex = endIdx;
            if (TryBuildLineFromStartToHover(_startIndex, endIdx, out var line))
                ReplaceSelection(line);
        }

        private void OnPointerUp(PuzzleTouchData data)
        {
            _isSelecting = false;
            _startIndex = -1;

            bool isValid = _puzzleView.ValidateWord(_sb.ToString(), GetReversedString(), out var foundWord,
                out var isReversed);
            this.LogInfo("OnPointerUp:", "isValid =", isValid, "word = ", _sb.ToString(), "selectedIndices =",
                _selectedIndices.ToArray(),
                "foundWord =", foundWord?.Word);

            if (isValid)
            {
                _selectionLineHandler.CompleteActiveLine();
                _puzzleView.AddFoundWord(foundWord);
                _targetWordsView.PlayAnimWordCompleted(_selectedIndices, foundWord.Word, isReversed,
                    OnPlayAnimCompleted);
                foreach (var idx in _selectedIndices)
                {
                    var cell = _builder.GetCellAtIndex(idx);
                    cell.PlayCompletedAnim();
                }
            }
            else
            {
                _selectedWordView.PlayIncorrectAnimation();
                _selectionLineHandler.EraseLine();
            }

            _selectedIndices.Clear();
            _currentCellIndex = -1;
            SyncHighlights();
        }

        private string GetReversedString()
        {
            var str = _sb.ToString();
            char[] array = str.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }

        private void ReplaceSelectionSingle(int idx)
        {
            _selectedIndices.Clear();
            _selectedIndices.Add(idx);

            var cell = _builder.GetCellAtIndex(idx);
            _selectionLineHandler.SetLineStart(cell);

            SyncHighlights();
        }

        private void ReplaceSelection(List<int> newSelection)
        {
            _selectedIndices.Clear();
            _sb.Clear();
            _selectedIndices.AddRange(newSelection);

            var cells = _builder.Cells;
            for (int i = 0; i < _selectedIndices.Count; i++)
            {
                var cell = cells[_selectedIndices[i]];
                _sb.Append(cell.Letter);
            }

            _selectionLineHandler.UpdateLine(_builder.GetCellAtIndex(_selectedIndices[^1]));
            _selectedWordView.SetWord(_sb.ToString());
            SyncHighlights();
        }

        private void SyncHighlights()
        {
            var cells = _builder.Cells;
            for (int i = 0; i < cells.Count; i++)
            {
                var cell = cells[i];
                bool shouldBeHighlighted =
                    _selectedIndices.Contains(cell.Index);
                cell.SetHighlighted(shouldBeHighlighted);
                cell.SetSelected(i == _currentCellIndex);
            }
        }

        /// <summary>
        /// Builds valid end candidates: all cells on the allowed direction rays from the start cell.
        /// Picks the candidate whose center is closest to the current pointer position (`local`).
        /// </summary>
        private bool TryGetClosestReachableEndFromStart(int startIdx, Vector2 pointerLocal, out int endIdx)
        {
            endIdx = -1;

            int cols = _level.Cols;
            var (sr, sc) = GamePuzzle.IndexToRC(startIdx, cols);

            float bestDistSqr = float.PositiveInfinity;
            bool found = false;

            // Include the start cell as a valid end.
            {
                Vector2 center = GetCellCenterLocal(sr, sc);
                float d = (pointerLocal - center).sqrMagnitude;
                bestDistSqr = d;
                endIdx = startIdx;
                found = true;
            }

            foreach (var dir in PuzzleBuilder.Directions)
            {
                // Skip zero direction if present for safety.
                if (dir.x == 0 && dir.y == 0)
                    continue;

                int rr = sr + dir.x;
                int cc = sc + dir.y;

                while (rr >= 0 && rr < _level.Rows && cc >= 0 && cc < _level.Cols)
                {
                    // Indices are kept in the builder/grid order. The grid's RectTransform rotation already
                    // affects pointerLocal (ScreenPointToLocalPointInRectangle), so we must not "re-orient"
                    // indices here (that would mirror selection).
                    int idx = rr * _level.Cols + cc;

                    Vector2 center = GetCellCenterLocal(rr, cc);
                    float d = (pointerLocal - center).sqrMagnitude;

                    if (d < bestDistSqr)
                    {
                        bestDistSqr = d;
                        endIdx = idx;
                        found = true;
                    }

                    rr += dir.x;
                    cc += dir.y;
                }
            }

            return found && endIdx >= 0;
        }

        public void ApplyOrientation(ref int row, ref int col, int rows, int cols,
            PuzzleView.BoardOrientation orientation)
        {
            switch (orientation)
            {
                case PuzzleView.BoardOrientation.Rot180:
                    row = (rows - 1) - row;
                    col = (cols - 1) - col;
                    break;
                case PuzzleView.BoardOrientation.Normal:
                default:
                    break;
            }
        }

        public static int TransformIndex(int index, int rows, int cols, PuzzleView.BoardOrientation orientation)
        {
            if (orientation == PuzzleView.BoardOrientation.Rot180)
            {
                int total = rows * cols;
                return (total - 1) - index;
            }

            return index;
        }

        public bool ValidateSelection(List<int> indices)
        {
            if (_level == null) return false;
            if (indices == null || indices.Count == 0) return false;

            int rows = _level.Rows;
            int cols = _level.Cols;

            for (int i = 0; i < indices.Count; i++)
            {
                int idx = indices[i];
                if (idx < 0 || idx >= _level.CellCount) return false;
            }

            if (indices.Count == 1) return true;

            var (r0, c0) = GamePuzzle.IndexToRC(indices[0], cols);
            var (r1, c1) = GamePuzzle.IndexToRC(indices[1], cols);

            int dr = r1 - r0;
            int dc = c1 - c0;

            bool isAllowed = false;
            foreach (var dir in PuzzleBuilder.Directions)
            {
                if (dir.x == dr && dir.y == dc)
                {
                    isAllowed = true;
                    break;
                }
            }

            if (!isAllowed) return false;

            int prevRow = r1;
            int prevCol = c1;

            for (int i = 2; i < indices.Count; i++)
            {
                var (ri, ci) = GamePuzzle.IndexToRC(indices[i], cols);

                if (ri != prevRow + dr || ci != prevCol + dc) return false;

                if (ri < 0 || ri >= rows || ci < 0 || ci >= cols) return false;

                if (dc != 0)
                {
                    int expectedCol = prevCol + dc;
                    if (expectedCol < 0 || expectedCol >= cols) return false;
                    if (ci != expectedCol) return false;
                }

                prevRow = ri;
                prevCol = ci;
            }

            return true;
        }

        public bool TryGetCellIndex(Vector2 screenPos, out int index)
        {
            index = -1;

            var grid = _builder.GridLayoutGroup;
            var rect = grid.transform as RectTransform;

            if (!RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, null))
                return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, null, out var local))
                return false;

            var r = rect.rect;
            var pad = grid.padding;
            var spacing = grid.spacing;
            var cell = grid.cellSize;

            var origin = new Vector2(r.xMin + pad.left, r.yMax - pad.top);
            var xSign = +1;
            var ySign = -1;

            float pitchX = cell.x + spacing.x;
            float pitchY = cell.y + spacing.y;

            float dx = (local.x - origin.x) * xSign;
            float dy = (local.y - origin.y) * ySign;

            if (dx < 0 || dy < 0) return false;

            int col = Mathf.FloorToInt(dx / pitchX);
            int row = Mathf.FloorToInt(dy / pitchY);

            if (_level == null) return false;
            if (col < 0 || col >= _level.Cols || row < 0 || row >= _level.Rows) return false;

            float xWithin = dx - col * pitchX;
            float yWithin = dy - row * pitchY;

            float cx = cell.x * 0.5f;
            float cy = cell.y * 0.5f;

            float rx = xWithin - cx;
            float ry = yWithin - cy;

            float radius = Mathf.Min(cell.x, cell.y) * _cellDetectRadiusFactor;
            float radiusSqr = radius * radius;

            if ((rx * rx + ry * ry) > radiusSqr)
                return false;

            // IMPORTANT:
            // The board is rotated visually by rotating the GridLayoutGroup RectTransform.
            // ScreenPointToLocalPointInRectangle already returns coordinates in that rotated local space,
            // so applying an additional index orientation flip here would mirror the mapping (the bug).
            // Therefore, we keep (row,col) as-is.

            index = row * _level.Cols + col;
            return true;
        }

        public bool TryGetCellAtScreenPos(Vector2 screenPos, out PuzzleCell cell)
        {
            cell = null;
            if (!TryGetCellIndex(screenPos, out int idx))
                return false;

            if (idx < 0 || idx >= _builder.Cells.Count) return false;
            cell = _builder.Cells[idx];
            return true;
        }

        // Add back missing helpers used by pointer move selection.
        private bool TryGetGridLocal(Vector2 screenPos, out Vector2 local)
        {
            local = default;

            var grid = _builder.GridLayoutGroup;
            if (grid == null)
                return false;

            var rect = grid.transform as RectTransform;
            if (rect == null)
                return false;

            if (!RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, null))
                return false;

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, null, out local);
        }

        private Vector2 GetCellCenterLocal(int row, int col)
        {
            var grid = _builder.GridLayoutGroup;
            var rect = grid.transform as RectTransform;

            var r = rect.rect;
            var pad = grid.padding;
            var spacing = grid.spacing;
            var cell = grid.cellSize;

            // Same origin convention as TryGetCellIndex: top-left of the content area.
            var origin = new Vector2(r.xMin + pad.left, r.yMax - pad.top);
            var xSign = +1;
            var ySign = -1;

            float pitchX = cell.x + spacing.x;
            float pitchY = cell.y + spacing.y;

            float x = origin.x + xSign * (col * pitchX + cell.x * 0.5f);
            float y = origin.y + ySign * (row * pitchY + cell.y * 0.5f);

            return new Vector2(x, y);
        }

        private List<int> _lineCache = new List<int>();

        private bool TryBuildLineFromStartToHover(int startIdx, int endIdx, out List<int> line)
        {
            line = null;
            if (startIdx == endIdx)
            {
                _lineCache.Clear();
                _lineCache.Add(startIdx);
                line = _lineCache;
                return true;
            }

            if (_level == null)
                return false;

            int cols = _level.Cols;

            var (sr, sc) = GamePuzzle.IndexToRC(startIdx, cols);
            var (er, ec) = GamePuzzle.IndexToRC(endIdx, cols);

            int dr = Math.Sign(er - sr);
            int dc = Math.Sign(ec - sc);

            // Validate direction is one of allowed directions (including diagonals).
            bool isAllowed = false;
            foreach (var dir in PuzzleBuilder.Directions)
            {
                if (dir.x == dr && dir.y == dc)
                {
                    isAllowed = true;
                    break;
                }
            }

            if (!isAllowed)
                return false;

            // Ensure end lies exactly on the ray from start.
            int deltaR = er - sr;
            int deltaC = ec - sc;

            if (dr == 0 && deltaR != 0) return false;
            if (dc == 0 && deltaC != 0) return false;
            if (dr != 0 && dc != 0 && Math.Abs(deltaR) != Math.Abs(deltaC)) return false;

            int steps = Math.Max(Math.Abs(deltaR), Math.Abs(deltaC));

            _lineCache.Clear();
            var result = _lineCache;
            int r = sr;
            int c = sc;
            for (int i = 0; i <= steps; i++)
            {
                int idx = r * _level.Cols + c;
                if (idx < 0 || idx >= _level.CellCount)
                    return false;

                result.Add(idx);
                r += dr;
                c += dc;
            }

            line = result;
            return true;
        }
    }
}