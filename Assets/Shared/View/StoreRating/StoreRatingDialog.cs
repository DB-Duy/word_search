using System.Collections;
using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Service.StoreRating;
using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Shared.View.StoreRating
{
    [DisallowMultipleComponent]
    public class StoreRatingDialog : IoCMonoBehavior, ISharedUtility
    {
        private const string Tag = "StoreRatingDialog";

        [Inject] private IStoreRatingService _service;
        
        [SerializeField] private List<StoreRatingStar> stars;
        [SerializeField] private AfterUserSelectType afterUserSelectType;
        [SerializeField] public UnityEvent<int> onUserSelectEvent;
        
        private bool _isListingUp = false;

        private void Start()
        {
            for (var i = 0; i < stars.Count; i++)
                stars[i].Button.onClick.AddListener(() =>
                {
                    if (_isListingUp) return;
                    _isListingUp = true;
                    StartCoroutine(_LightUp(i));
                });
        }

        private void OnEnable()
        {
            this.Track(TrackingScreen.FakeRating);
            foreach (var t in stars) t.LightOff();
            _isListingUp = false;
            _service.IncreaseShownCountByOne();
        }

        private IEnumerator _LightUp(int starIndex)
        {
            for (var i = 0; i < stars.Count; i++)
            {
                stars[i].SetLightState(isLit: i < starIndex);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(2f);
            if (starIndex < 4)
            {
                _isListingUp = false;
                onUserSelectEvent.Invoke(starIndex);
                _HandleAfterUserSelect();
                yield break;
            }
            _service.Rate();
            onUserSelectEvent.Invoke(starIndex);
            _HandleAfterUserSelect();
        }

        private void _HandleAfterUserSelect()
        {
            switch (afterUserSelectType)
            {
                case AfterUserSelectType.Hide:
                    gameObject.SetActive(false);
                    break;
                case AfterUserSelectType.Destroy:
                    Destroy(gameObject);
                    break;
            }
        }

        private enum AfterUserSelectType
        {
            None,
            Hide,
            Destroy
        }
    }
}
