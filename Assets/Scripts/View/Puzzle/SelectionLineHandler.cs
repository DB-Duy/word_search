// Assets/Scripts/View/Puzzle/SelectionLineHandler.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace View.Puzzle
{
    public class SelectionLineHandler
    {
        private readonly ObjectPool<SelectionLine> _linePool;

        private static List<Color> _lineColors = new List<Color>
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            new Color(1f, 0.5f, 0f),
            new Color(0.5f, 0f, 1f),
        };

        private List<SelectionLine> _lines = new List<SelectionLine>();
        private SelectionLine _activeLine;
        private PuzzleCell _lineStartCell;

        // Tune this to land on the text nicely (in the same UI units as your cells).
        private const float StartTextOffset = 12f;

        public SelectionLineHandler(ObjectPool<SelectionLine> linePool)
        {
            _linePool = linePool;
        }

        public void SetLineStart(PuzzleCell startCell)
        {
            _lineStartCell = startCell;
            _activeLine = _linePool.Get();
            _activeLine.SetColor(_lineColors[Random.Range(0, _lineColors.Count)]);
            _activeLine.SetStartCap(startCell.transform.position);
        }

        public void UpdateLine(PuzzleCell currentCell)
        {
            if (_activeLine == null || _lineStartCell == null || currentCell == null) return;
            _activeLine.SetEndCap(currentCell.transform.position);
        }
        
        public void FinalizeLine(bool isValid)
        {
            if (_activeLine == null) return;
            if (isValid)
            {
                _lines.Add(_activeLine);
            }
            else
            {
                _linePool.Release(_activeLine);
            }

            _activeLine = null;
            _lineStartCell = null;
        }

        public void ClearSelectedLine()
        {
            if (_activeLine != null)
            {
                _linePool.Release(_activeLine);
                _activeLine = null;
            }

            _lineStartCell = null;
        }
    }
}