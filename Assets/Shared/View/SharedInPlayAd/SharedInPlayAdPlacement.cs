using System;
using Shared.Controller.SharedInPlayAd;
using Shared.Core.IoC;
using UnityEngine;
using Zenject;

namespace Shared.View.SharedInPlayAd
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class SharedInPlayAdPlacement : IoCMonoBehavior
    {
        [Inject] private ISharedInPlayAdController _controller;
        private string[] _priorityAds;
        private float _duration;
        private float _totalTime;

        private IState _waitingState = new WaitingState();
        private IState _refreshState = new RefreshState();
        private IState _currentState;
        private GameObject _currentAd;
        
        private void OnEnable()
        {
            // _controller.Request<>();
        }

        private void Update() => _currentState?.Update();

        private void OnDisable()
        {
            throw new NotImplementedException();
        }

        private void OnDestroy()
        {
            throw new NotImplementedException();
        }

        private GameObject _Request()
        {
            var foundOne = _controller.RequestByClassName(_priorityAds);
            if (foundOne == null) return null;
            
            foundOne.transform.SetParent(transform);
            foundOne.transform.position = transform.position;
            _currentAd = foundOne;
            return foundOne;
        }

        private interface IState
        {
            void Update();
        }
        
        private class WaitingState : IState
        {
            private float _totalTime = 0;
            public float Duration { get; set; }
            public SharedInPlayAdPlacement Placement { get; set; }

            public void Update()
            {
                _totalTime += Time.unscaledDeltaTime;
                if (!(_totalTime >= Duration)) return;
                _totalTime = 0f;
                var r = Placement._Request();
                if (r != null) Placement._currentState = Placement._refreshState;
            }
        }

        private class RefreshState : IState
        {
            private float _totalTime = 0;
            public float Duration { get; set; }
            public SharedInPlayAdPlacement Placement { get; set; }
            
            public void Update()
            {
                _totalTime += Time.unscaledDeltaTime;
                if (!(_totalTime >= Duration)) return;
                _totalTime = 0f;
                var r = Placement._Request();
                if (r != null) Placement._currentState = Placement._refreshState;
            }
        }
    }
}