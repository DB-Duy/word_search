// Assets/Scripts/View/Puzzle/SelectionLineHandler.cs

using System.Collections.Generic;
using System.Linq;
using Service.Puzzle;
using UnityEngine;
using UnityEngine.Pool;

namespace View.Puzzle
{
    public class SelectionLineHandler
    {
        private readonly ObjectPool<SelectionLine> _linePool;

        private static readonly List<Color> _lineColors = new List<Color>
        {
            new Color(0.85f, 0.33f, 0.31f), // soft red
            new Color(0.30f, 0.55f, 0.85f), // soft blue
            new Color(0.35f, 0.75f, 0.45f), // soft green
            new Color(0.95f, 0.65f, 0.25f), // soft orange
            new Color(0.60f, 0.45f, 0.85f), // soft purple
            new Color(0.95f, 0.50f, 0.70f), // soft pink
            new Color(0.40f, 0.80f, 0.75f), // soft teal
            new Color(0.75f, 0.75f, 0.35f), // muted yellow
            new Color(0.90f, 0.55f, 0.55f),
            new Color(0.55f, 0.70f, 0.90f),
            new Color(0.60f, 0.85f, 0.65f),
            new Color(0.95f, 0.80f, 0.50f),
            new Color(0.75f, 0.65f, 0.90f),
            new Color(0.95f, 0.65f, 0.80f),
        };

        private List<SelectionLine> _lines = new List<SelectionLine>();
        private SelectionLine _activeLine;
        private PuzzleCell _lineStartCell;
        private HashSet<Color> _availableColors = new HashSet<Color>();
        private Dictionary<TargetWord, Color> _wordColors = new();

        // Tune this to land on the text nicely (in the same UI units as your cells).
        private const float StartTextOffset = 12f;

        public SelectionLineHandler(ObjectPool<SelectionLine> linePool)
        {
            _linePool = linePool;
        }

        public void RefreshColors()
        {
            _availableColors.Clear();
            foreach (var color in _lineColors)
            {
                _availableColors.Add(color);
            }
        }

        private Color GetRandomColor()
        {
            if (_availableColors.Count == 0)
            {
                foreach (var color in _lineColors)
                {
                    _availableColors.Add(color);
                }
            }

            int index = Random.Range(0, _availableColors.Count);

            int i = 0;
            foreach (var item in _availableColors)
            {
                if (i == index)
                    return item;
                i++;
            }

            return _availableColors.First();
        }


        public void SetLineStart(PuzzleCell startCell)
        {
            _lineStartCell = startCell;
            _activeLine = _linePool.Get();
            var c = GetRandomColor();
            _activeLine.SetColor(c);
            _activeLine.SetStartCap(startCell.transform.position);
        }

        public void UpdateLine(PuzzleCell currentCell)
        {
            if (_activeLine == null || _lineStartCell == null || currentCell == null) return;
            _activeLine.SetEndCap(currentCell.transform.position);
        }

        public Color GetCurrentLineColor()
        {
            return _activeLine != null ? _activeLine.Color : Color.clear;
        }

        public void FinalizeLine(bool isValid, TargetWord word)
        {
            if (_activeLine == null) return;
            if (isValid)
            {
                _lines.Add(_activeLine);
                _wordColors[word] = _activeLine.Color;
                _availableColors.Remove(_activeLine.Color);
                _activeLine.PlayLineComplete();
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