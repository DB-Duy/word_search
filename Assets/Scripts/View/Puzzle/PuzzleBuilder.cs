using System.Collections;
using System.Collections.Generic;
using Service.Puzzle;
using Shared;
using Shared.Service.SharedCoroutine;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace View.Puzzle
{
    public class PuzzleBuilder : ISharedUtility
    {
        private ObjectPool<PuzzleCell> _cellPool;
        private GamePuzzle _level;
        private GridLayoutGroup _gridLayoutGroup;
        private List<PuzzleCell> cells = new List<PuzzleCell>();
        public List<PuzzleCell> Cells => cells;
        public GridLayoutGroup GridLayoutGroup => _gridLayoutGroup;

        public PuzzleBuilder(ObjectPool<PuzzleCell> cellPool, GridLayoutGroup gridLayoutGroup)
        {
            _cellPool = cellPool;
            _gridLayoutGroup = gridLayoutGroup;
        }

        // (dr, cd)
        public static readonly Vector2Int[] Directions = new[]
        {
            new Vector2Int(0, 1), // Right
            new Vector2Int(0, -1), // Left
            new Vector2Int(1, 0), // Down
            new Vector2Int(-1, 0), // Up
            new Vector2Int(1, 1), // Down-Right
            new Vector2Int(1, -1), // Down-Left
            new Vector2Int(-1, 1), // Up-Right
            new Vector2Int(-1, -1), // Up-Left
        };

        public PuzzleCell GetCellAtIndex(int index)
        {
            if (index < 0 || index >= Cells.Count) return null;
            return Cells[index];
        }

        public void SetupPuzzle(GamePuzzle level)
        {
            _level = level;

            _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayoutGroup.constraintCount = level.Cols;

            foreach (var cell in cells)
            {
                _cellPool.Release(cell);
            }

            cells.Clear();

            // Create cells and fill letters
            for (int idx = 0; idx < level.CellCount; idx++)
            {
                var cell = _cellPool.Get();
                var (row, col) = GamePuzzle.IndexToRC(idx, level.Cols);

                cell.Index = idx;
                cell.SetLetter(level.LetterAt(idx));

                // bool isDead = level.DeadLetterIndices.Contains(idx);
                // cell.SetDead(isDead);

                cells.Add(cell);
            }

            ScaleCellSizeToFit();
        }

        private void ScaleCellSizeToFit()
        {
            this.StartSharedCoroutine(DelayScaleCells());

            IEnumerator DelayScaleCells()
            {
                yield return new WaitForEndOfFrame();

                var rectTransform = (RectTransform)_gridLayoutGroup.transform;
                var parentRect = rectTransform.rect;

                int cols = Mathf.Max(1, _level.Cols);
                int rows = Mathf.Max(1, _level.Rows);

                float totalSpacingX = _gridLayoutGroup.spacing.x * Mathf.Max(0, cols - 1);
                float totalSpacingY = _gridLayoutGroup.spacing.y * Mathf.Max(0, rows - 1);

                float availableWidth = parentRect.width - totalSpacingX;
                float availableHeight = parentRect.height - totalSpacingY;

                float cellWidth = availableWidth / cols;
                float cellHeight = availableHeight / rows;

                _gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            }
        }


        private List<int> FindWordPath(string word, int startIndex, int rows, int cols)
        {
            var (startRow, startCol) = GamePuzzle.IndexToRC(startIndex, cols);

            foreach (var directions in Directions)
            {
                var indices = new List<int>();
                int row = startRow;
                int col = startCol;

                bool ok = true;
                for (int k = 0; k < word.Length; k++)
                {
                    if (row < 0 || row >= rows || col < 0 || col >= cols)
                    {
                        ok = false;
                        break;
                    }

                    int idx = GamePuzzle.RCToIndex(row, col, cols);
                    if (_level.LetterAt(idx) != word[k])
                    {
                        ok = false;
                        break;
                    }

                    indices.Add(idx);
                    row += directions.x;
                    col += directions.y;

                    // Prevent wrapping horizontally when moving left/right/diagonals
                    if (k < word.Length - 1 && directions.y != 0)
                    {
                        int prevCol = GamePuzzle.IndexToRC(indices[^1], cols).col;
                        int nextCol = prevCol + directions.y;
                        if (nextCol < 0 || nextCol >= cols)
                        {
                            ok = false;
                            break;
                        }
                    }
                }

                if (ok) return indices;
            }

            return null;
        }
    }
}