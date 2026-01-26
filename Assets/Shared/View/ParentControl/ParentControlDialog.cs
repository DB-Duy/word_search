using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.View.Dialog;
using Shared.Core.View.Scene;
using Shared.Entity.ParentControl;
using Shared.Service.ParentControl;
using Shared.Utils;
using Shared.View.EnhancedScroll;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.ParentControl
{
    [DisallowMultipleComponent]
    public class ParentControlDialog : IoCMonoBehavior, IUIDialog, ISharedUtility, IEnhancedScrollerDelegate
    {
        [System.Serializable]
        public class UserInfoGroup
        {
            [SerializeField] internal GameObject rootObject;
            
            [SerializeField] internal EnhancedScroller yearScroller;
            [SerializeField] internal ScrollRect yearScrollRect;
            [SerializeField] internal YearItem yearItemPrefab;
            [SerializeField] internal Color normalYearTextColor = new (0.28f, 0.5f, 0.9f, 1f);
            [SerializeField] internal Color selectedYearTextColor = Color.white;
            [SerializeField] internal Image yearScrollMask;
        
            [SerializeField] internal ToggleGroup genderGroup;
            [SerializeField] internal List<Toggle> genderToggles;
            
            [SerializeField] internal Button okButton;
            
            internal SmallList<YearData> YearList;
            internal int SelectedYear;
        }
        
        [System.Serializable]
        public class PolicyGroup
        {
            [SerializeField] internal GameObject rootObject;
            [SerializeField] internal Button acceptButton;
        }
        
        [System.Serializable]
        public class ParentGroup
        {
            [SerializeField] internal GameObject rootObject;
            [SerializeField] internal Button grantButton;
        }

        [SerializeField] private UserInfoGroup userInfoGroup;
        [SerializeField] private PolicyGroup policyGroup;
        [SerializeField] private ParentGroup parentGroup;

        [Inject] private IParentControlService _parentControlService;
        [Inject(Optional = true)] private IParentControlAudioPlayer _audioPlayer;

        private void Start()
        {
            userInfoGroup.okButton.onClick.AddListener(_OnUserInfoGroupButtonClicked);
            policyGroup.acceptButton.onClick.AddListener(_OnPolicyGroupButtonClicked);
            parentGroup.grantButton.onClick.AddListener(_OnParentGroupButtonClicked);
        }

        private void OnEnable()
        {
            var currentStep = _parentControlService.GetStep();
            this.LogInfo(nameof(currentStep), currentStep.ToString());

            var isUserInfoStep = currentStep is Step.AskUserInfo or Step.NotStarted;
            userInfoGroup.rootObject.SetActive(isUserInfoStep);
            policyGroup.rootObject.SetActive(currentStep is Step.Policy);
            parentGroup.rootObject.SetActive(currentStep is Step.ParentPermission);
            if (isUserInfoStep)
            {
                _SetupUserInfoGroup();
                StartCoroutine(_RefreshUserInfoAfterSomeFrames());
            }
        }
        
        private IEnumerator _RefreshUserInfoAfterSomeFrames()
        {
            yield return null;
            yield return null;
            yield return null;
            _RefreshUserInfoGroup();
        }

        private void _OnUserInfoGroupButtonClicked()
        {
            this.LogInfo(SharedLogTag.ParentControl);
            _audioPlayer?.PlayButtonClickSound();
            _parentControlService.SetStep(Step.Policy);
            userInfoGroup.rootObject.SetActive(false);
            policyGroup.rootObject.SetActive(true);
            parentGroup.rootObject.SetActive(false);
        }

        private void _OnPolicyGroupButtonClicked()
        {
            _audioPlayer?.PlayButtonClickSound();
            userInfoGroup.rootObject.SetActive(false);
            
            var nextStep = _parentControlService.EvaluateNextStep();
            if (nextStep == Step.ParentPermission)
            {
                _parentControlService.SetStep(Step.ParentPermission);
                policyGroup.rootObject.SetActive(false);
                parentGroup.rootObject.SetActive(true);
                return;
            }
            _parentControlService.SetStep(nextStep);
            this.HideDialog(this);
        }

        private void _OnParentGroupButtonClicked()
        {
            _audioPlayer?.PlayButtonClickSound();
            _parentControlService.SetStep(Step.Granted);
            this.HideDialog(this);
        }

        private void _SetupUserInfoGroup()
        {
            userInfoGroup.SelectedYear = _parentControlService.GetYear();
            
            userInfoGroup.okButton.interactable = false;
            userInfoGroup.yearScroller.Delegate = this;
            
            userInfoGroup.YearList = new SmallList<YearData>();
            var yearList = _parentControlService.GetYears();
            yearList.ForEach(y => userInfoGroup.YearList.Add(new YearData(y)));
            
            var gender = _parentControlService.GetGender();
            if (gender != Gender.None)
            {
                userInfoGroup.genderToggles[(int)gender].isOn = true;
                userInfoGroup.genderGroup.allowSwitchOff = false;
                userInfoGroup.okButton.interactable = true;
            }
            else
            {
                userInfoGroup.genderGroup.allowSwitchOff = true;
            }

            userInfoGroup.genderToggles.SetOnValueChangedAction((index, isOn) =>
            {
                if (!isOn) return;
                _audioPlayer?.PlayButtonClickSound();
                userInfoGroup.genderGroup.allowSwitchOff = false;
                userInfoGroup.okButton.interactable = true;
                _parentControlService.SaveGender((Gender)index);
            });
            
        }

        private void _RefreshUserInfoGroup()
        {
            userInfoGroup.yearScroller.ReloadData();

            var yearList = _parentControlService.GetYears();
            var selectedYear = _parentControlService.GetYear();
            var indexOfSelectedYear = yearList.IndexOf(selectedYear);
            if (indexOfSelectedYear > 0) indexOfSelectedYear--;
            userInfoGroup.yearScroller.JumpToDataIndex(indexOfSelectedYear, tweenTime: 0.5f, loopJumpDirection: EnhancedScroller.LoopJumpDirectionEnum.Closest);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Year Scroll
        // -------------------------------------------------------------------------------------------------------------
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return userInfoGroup.YearList.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 200f;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var yearItem = scroller.GetCellView(userInfoGroup.yearItemPrefab) as YearItem;
            yearItem.Year = userInfoGroup.YearList[dataIndex].Year;
            yearItem.OnClickedAction = OnYearItemOnClickedAction;
            yearItem.OnRefreshAction = OnYearItemRefreshedAction;
            yearItem.Refresh(userInfoGroup.SelectedYear, userInfoGroup.normalYearTextColor, userInfoGroup.selectedYearTextColor);
            return yearItem;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Year Item Actions
        // -------------------------------------------------------------------------------------------------------------
        private Action<YearItem> _onYearItemClickedAction;
        private Action<YearItem> OnYearItemOnClickedAction => _onYearItemClickedAction ??= yearItem =>
        {
            _audioPlayer?.PlayButtonClickSound();
            _parentControlService.SaveYear(yearItem.Year);
            userInfoGroup.SelectedYear = yearItem.Year;
            StartCoroutine(_FocusOnYearItem(yearItem));
        };
        
        private IEnumerator _FocusOnYearItem(YearItem yearItem)
        {
            userInfoGroup.yearScroller.RefreshActiveCellViews();
            userInfoGroup.yearScrollMask.raycastTarget = true;
            var itemRectTransform = yearItem.GetComponent<RectTransform>();
            yield return userInfoGroup.yearScrollRect.FocusOnItemCoroutine(itemRectTransform, 2f);
            userInfoGroup.yearScrollMask.raycastTarget = false;
        }
        
        private Action<YearItem> _onYearItemRefreshedAction;
        private Action<YearItem> OnYearItemRefreshedAction => _onYearItemRefreshedAction ??= yearItem =>
        {
            yearItem.Refresh(userInfoGroup.SelectedYear, userInfoGroup.normalYearTextColor, userInfoGroup.selectedYearTextColor);
        };
    }

    public static class ParentControlDialogExtensions
    {
        public static void SetOnValueChangedAction(this List<Toggle> me, Action<int, bool> onValueChanged)
        {
            for (var i = 0; i < me.Count; i++)
            {
                var i1 = i;
                me[i].onValueChanged.RemoveAllListeners();
                me[i].onValueChanged.AddListener(isOn => onValueChanged(i1, isOn));
            }
        }

        public static void Refresh(this YearItem me, int selectedYear, Color normalYearTextColor, Color selectedYearTextColor)
        {
            var isSelected = me.Year == selectedYear;
            me.YearTextColor = isSelected ? selectedYearTextColor : normalYearTextColor;
            me.IsBackgroundActive = isSelected;
        }
    }
}