using System;
using Shared.View.EnhancedScroll;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.ParentControl
{
    [DisallowMultipleComponent]
    public class YearItem : EnhancedScrollerCellView
    {
        [SerializeField] private GameObject background;
        [SerializeField] private TextMeshProUGUI yearText;
        [SerializeField] private Button button;

        public Action<YearItem> OnRefreshAction { get; set; }
        public Action<YearItem> OnClickedAction { get; set; }

        private int _year;
        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                yearText.text = value + "";
            }
        }

        public Color YearTextColor
        {
            set => yearText.color = value;
        }

        public bool IsBackgroundActive
        {
            set => background.SetActive(value);
        }
        
        private void Start()
        {
            button.onClick.AddListener(() => OnClickedAction?.Invoke(this));
        }
        
        public override void RefreshCellView() => OnRefreshAction?.Invoke(this);
    }
}