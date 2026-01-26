#if FACEBOOK_INSTANT
using System;
using Game.Scripts.Data;
using Shared.IAP.Config;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.IAP.FacebookInstant
{
    public class FacebookInstantIapController : MonoBehaviour, IIapController
    {
        private const string TAG = "FacebookInstantIapController";
        private static FacebookInstantIapController _instance;
        private bool _isInitialized;
        public IIapConfig Config { get; }
        
        public void Add(params ISilentPurchaseCompleteEventHandler[] handlers)
        {
            
        }

        public IIapController AddSubscriptionMonitors(params ISubscriptionMonitor[] monitors)
        {
            return this;
        }

        public Action OnInitializedSuccess { get; set; }

        public IIapController SetConfig(IIapConfig config)
        {
            return this;
        }

        bool IInitializable.IsInitialized => _isInitialized;

        public IAsyncOperation Initialize()
        {
            return new SharedAsyncOperation().Success();
        }

        public bool IsInitialized()
        {
            return true;
        }

        public IIapPurchasingAsyncOperation Purchase(string id)
        {
            return IapPurchasingAsyncOperation.SuccessInstance(id);
        }

        public void ProcessPurchaseComplete(string id)
        {
            
        }

        public void RestorePurchases()
        {
            
        }

        public bool IsSubscriptionId(string id)
        {
            return false;
        }

        public bool ValidateSubscription(string id)
        {
            return GameCore.Instance.IsPremiumEnable.Get();
        }

        public void DebugSubscriptionInfo(string id)
        {
        }
        
        public IIapProduct GetProduct(string id)
        {
            return null;
        }

        public void RedirectingToSubscriptionManagementScreen(string productId = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif