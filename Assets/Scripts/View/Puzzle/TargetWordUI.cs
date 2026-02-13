using TMPro;
using UnityEngine;

namespace View.Puzzle
{
    public class TargetWordUI : MonoBehaviour
    {
        private const string CompletedWordFormat = "<color=#ABABAB>{0}</color>";

        [SerializeField] private TextMeshProUGUI _targetWordText;

        public float FontSize
        {
            get => _targetWordText.fontSize;
            set => _targetWordText.fontSize = value;
        }

        public void SetText(string word)
        {
            _targetWordText.SetText(word);
        }
        
        public TMP_TextInfo TextInfo
        {
            get => _targetWordText.textInfo;
        }
    }
}