using TMPro;
using UnityEngine;

namespace View.Puzzle
{
    public class TargetWordUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _targetWordText;
        private static Color CompletedColor = new Color(0.67f, 0.67f, 0.67f);
        private string _word;
        public string Word
        {
            get => _word;
        }
        public float FontSize
        {
            get => _targetWordText.fontSize;
            set => _targetWordText.fontSize = value;
        }

        public void SetText(string word)
        {
            _word = word;
            _targetWordText.SetText(word);
        }
        public void SetCompleted(bool completed)
        {
            _targetWordText.color = completed ? CompletedColor : Color.black;
        }
        
        public TMP_TextInfo TextInfo
        {
            get => _targetWordText.textInfo;
        }
    }
}