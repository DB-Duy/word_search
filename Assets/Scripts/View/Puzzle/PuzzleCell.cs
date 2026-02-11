using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.Puzzle
{
    public class PuzzleCell : MonoBehaviour
    {
        [Header("UI")] public TextMeshProUGUI tmpText; // Use TMP_Text if using TextMeshPro

        [Header("State")] public int Index;
        public char Letter;
        public bool IsDead;

        [Header("Styles")] public Color normalColor = Color.black;
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private Image _bgImage;
        private Sequence _highlightSequence;
        private bool _isHighlighted;
        public float FontSize => tmpText.fontSize;

        public void SetLetter(char c)
        {
            Letter = c;
            tmpText.SetText(c.ToString());
        }

        private void OnEnable()
        {
            _isHighlighted = true;
            SetHighlighted(false, true);
        }

        public void SetHighlighted(bool highlighted, bool immediate = false)
        {
            if (highlighted == _isHighlighted) return;
            _isHighlighted = highlighted;
            if (immediate)
            {
                tmpText.color = highlighted ? highlightColor : normalColor;
                tmpText.transform.localScale = highlighted ? Vector3.one * 1.3f : Vector3.one;
            }
            else
            {
                if (_highlightSequence.isAlive)
                {
                    _highlightSequence.Stop();
                }

                _highlightSequence = Sequence.Create()
                    .Group(Tween.Color(tmpText, highlighted ? highlightColor : normalColor, 0.2f))
                    .Group(Tween.Scale(tmpText.transform, Vector3.one * (highlighted ? 1.3f : 1f), 0.2f));
            }
        }
    }
}