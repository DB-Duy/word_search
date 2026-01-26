#if HUAWEI
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using HmsPlugin;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using Newtonsoft.Json;
using Shared.Common;
using Shared.IAP.Common;
using Shared.IAP.Config;
using Shared.IAP.Handlers;
using Shared.Utils;
using UnityEngine;

namespace Shared.IAP.HuaweiIap
{
    /// <summary>
    /// https://evilminddevs.gitbook.io/hms-unity-plugin/kits-and-services/in-app-purchases/guides-and-references
    /// https://github.com/EvilMindDevs/hms-unity-plugin/blob/master/Assets/Huawei/Demos/Game/GameDemoManager.cs
    /// </summary>
    [DisallowMultipleComponent]
    public class HuaweiIapController : MonoBehaviour, IHuaweiIapController
    {
        private const string TAG = "HuaweiIapController";
        public IIapConfig Config { get; private set; }
        
        public bool IsInitialized { get; private set; } = false;
        private IAsyncOperation _initOperation;
        private IIapPurchasingAsyncOperation _purchasingAsyncOperation;
        
        // Plugins
        private readonly ISilentPurchaseCompleteEventEngine _productSilentExecutorEngine = new SilentPurchaseCompleteCompleteEventEngine();
        private readonly ISubscriptionMonitorEngine _subscriptionMonitorEngine = new SubscriptionMonitorEngine();

        private HuaweiApplicationResumeAction _applicationResumeAction;
        private HuaweiCachedLastSubscriptionStateDict _cachedLastSubscriptionStateDict;
        
        private void Update()
        {
            // Every 60 frames, we monitor subscriptions.
            if (Time.frameCount % 60 == 0 && IsInitialized) _subscriptionMonitorEngine.Execute();
            if (_applicationResumeAction != null && !_applicationResumeAction.IsExecuted && Time.realtimeSinceStartup > _applicationResumeAction.Time) _applicationResumeAction.Invoke();
        }

        public Action OnInitializedSuccess { get; set; }

        public IIapController SetConfig(IIapConfig config)
        {
            Config = config;
            return this;
        }
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _cachedLastSubscriptionStateDict = new HuaweiCachedLastSubscriptionStateDict(this);
            HMSIAPManager.Instance.OnInitializeIAPSuccess += _OnInitializeIAPSuccess;
            HMSIAPManager.Instance.OnInitializeIAPFailure += _OnInitializeIAPFailure;
            HMSIAPManager.Instance.OnBuyProductSuccess += _OnBuyProductSuccess;
            HMSIAPManager.Instance.OnBuyProductFailure += _OnBuyProductFailure;
            HMSIAPManager.Instance.OnBuyProductFailurePurchaseResultInfo += _OnBuyProductFailurePurchaseResultInfo;

            HMSIAPManager.Instance.OnObtainOwnedPurchaseRecordSuccess += _OnObtainOwnedPurchaseRecordSuccess;
            HMSIAPManager.Instance.OnObtainOwnedPurchaseRecordFailure += _OnObtainOwnedPurchaseRecordFailure;
            
            HMSIAPManager.Instance.OnObtainOwnedPurchasesSuccess += _OnObtainOwnedPurchasesSuccess;
            HMSIAPManager.Instance.OnObtainOwnedPurchasesFailure += _OnObtainOwnedPurchasesFailure;
            
            HMSIAPManager.Instance.InitializeIAP();
            _initOperation = new SharedAsyncOperation();
            return _initOperation;
        }
        
        private void _OnInitializeIAPSuccess()
        {
            SharedLogger.Log($"{TAG}->_OnInitializeIAPSuccess");
            StartCoroutine(_CR_InitializeIAPSuccess());
        }

        private IEnumerator _CR_InitializeIAPSuccess()
        {
            SharedLogger.Log($"{TAG}->_CR_InitializeIAPSuccess");
            var asyncOperation = _RestorePurchases();
            yield return new WaitUntil(() => asyncOperation.IsComplete);
            
            IsInitialized = true;
            _subscriptionMonitorEngine.Execute();
            _initOperation.Success();
            OnInitializedSuccess?.Invoke();
        }

        private void _OnInitializeIAPFailure(HMSException ex)
        {
            SharedLogger.Log($"{TAG}->_OnInitializeIAPFailure");
            IsInitialized = false;
            _initOperation.Fail(ex.WrappedExceptionMessage);
            _initOperation = null; // Allow re-init
        }
        
        public void Add(params ISilentPurchaseCompleteEventHandler[] handlers)
        {
            _productSilentExecutorEngine.Register(handlers);
        }

        public IIapController AddSubscriptionMonitors(params ISubscriptionMonitor[] monitors)
        {
            foreach (var monitor in monitors) monitor.SetIapController(this);
            _subscriptionMonitorEngine.Add(monitors);
            return this;
        }

        public IIapPurchasingAsyncOperation Purchase(string id)
        {
            if (!IsInitialized) return (IapPurchasingAsyncOperation) new IapPurchasingAsyncOperation(id).Fail("!IsInitialized");
            if (_purchasingAsyncOperation != null) return (IapPurchasingAsyncOperation) new IapPurchasingAsyncOperation(id).Fail($"_purchasingAsyncOperation({_purchasingAsyncOperation.Id}) is executing.");
            _purchasingAsyncOperation = new IapPurchasingAsyncOperation(id);
            HMSIAPManager.Instance.PurchaseProduct(id, consume: false);
            return _purchasingAsyncOperation;
        }
        
        // {"ReturnCode":0,"ErrMsg":"success","InAppPurchaseData":{"ApplicationId":"109730591","AutoRenewing":true,"OrderID":"1713774484048.02E931BD.7627","PackageName":"com.indiez.nonogram.huawei","ProductId":"com.indiez.nonogram.premiumpack","ProductName":"Premium Pack","PurchaseTime":1713774484191,"PurchaseState":0,"DeveloperPayload":"","PurchaseToken":"0000018ef47e9e748dae9178e3ee0676c2f9ca17d19fc75cd678c95d0ce368e32c1d9c407d894703x564e.5.7627","PurchaseType":0,"Currency":"VND","Price":4900000,"Country":"VN","LastOrderId":"1713522697844.DE8031FC.7627","ProductGroup":"D4757C5A65254836A2C77E7D8C25EB87","OriPurchaseTime":1713499054904,"SubscriptionId":"1713498922612.04A7FC9F.7627","Quantity":1,"DaysLasted":17,"NumOfPeriods":18,"NumOfDiscount":0,"ExpirationDate":1713774664191,"ExpirationIntent":-2147483648,"RetryFlag":1,"IntroductoryFlag":0,"TrialFlag":0,"CancelTime":-2147483648,"CancelReason":-2147483648,"AppInfo":null,"NotifyClosed":-2147483648,"RenewStatus":1,"PriceConsentS
        private void _OnBuyProductSuccess(PurchaseResultInfo obj)
        {
            _DebugPurchaseResultInfo("_OnBuyProductSuccess", obj);
            ProcessPurchaseComplete(obj.InAppPurchaseData.ProductId);
        }

        public void ProcessPurchaseComplete(string id)
        {
            SharedLogger.Log($"{TAG}->ProcessPurchaseComplete: {id}");
            var errorMessage = string.Empty;
            if (string.IsNullOrEmpty(id)) errorMessage = "string.IsNullOrEmpty(id)";
            
            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.LogError($"{TAG}->ProcessPurchaseComplete: Error - {errorMessage}");
                return;
            }

            if (Config.ValidateSubscriptionsProduct(id)) StartCoroutine(_ProcessSubscriptionPurchaseComplete(id));
            else
            {
                if (_purchasingAsyncOperation == null)
                {
                    SharedLogger.Log($"{TAG}->ProcessPurchaseComplete: _productSilentExecutorEngine?.Execute({id});");
                    _productSilentExecutorEngine?.Execute(id);
                    return;
                }

                if (_purchasingAsyncOperation.Id != id)
                {
                    SharedLogger.Log(
                        $"{TAG}->ProcessPurchaseComplete::{id} != {_purchasingAsyncOperation.Id}, _productSilentExecutorEngine?.Execute({id});");
                    _productSilentExecutorEngine?.Execute(id);
                    return;
                }
                
                _purchasingAsyncOperation?.Success();
                _purchasingAsyncOperation = null;
                _applicationResumeAction = null;
            }
        }

        private IEnumerator _ProcessSubscriptionPurchaseComplete(string id)
        {
            if (_purchasingAsyncOperation == null)
            {
                SharedLogger.Log($"{TAG}->ProcessPurchaseComplete: _productSilentExecutorEngine?.Execute({id});");
                _productSilentExecutorEngine?.Execute(id);
                yield break;
            }

            if (_purchasingAsyncOperation.Id != id)
            {
                SharedLogger.Log($"{TAG}->ProcessPurchaseComplete::{id} != {_purchasingAsyncOperation.Id}, _productSilentExecutorEngine?.Execute({id});");
                _productSilentExecutorEngine?.Execute(id);
                yield break;
            }

            RestorePurchases();
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => ValidateSubscription(id));
            
            _subscriptionMonitorEngine.Execute();
            _purchasingAsyncOperation?.Success();
            _purchasingAsyncOperation = null;
            _applicationResumeAction = null;
        }


        //https://developer.huawei.com/consumer/en/doc/HMSCore-References/client-error-code-0000001050746111
        private void _OnBuyProductFailure(int errorCode)
        {
            SharedLogger.Log($"{TAG}->_OnBuyProductFailure: errorCode={errorCode}");
            _purchasingAsyncOperation?.Fail($"{TAG}->_OnBuyProductFailure: errorCode={errorCode}");
            _purchasingAsyncOperation = null;
            _applicationResumeAction = null;
        }

        // {"ReturnCode":60000,"ErrMsg":"cancel","InAppPurchaseData":{"ApplicationId":"","AutoRenewing":false,"OrderID":"","PackageName":null,"ProductId":"","ProductName":null,"PurchaseTime":-2147483648,"PurchaseState":0,"DeveloperPayload":null,"PurchaseToken":"","PurchaseType":-2147483648,"Currency":"","Price":0,"Country":"","LastOrderId":null,"ProductGroup":null,"OriPurchaseTime":-2147483648,"SubscriptionId":null,"Quantity":-2147483648,"DaysLasted":-2147483648,"NumOfPeriods":-2147483648,"NumOfDiscount":-2147483648,"ExpirationDate":-2147483648,"ExpirationIntent":-2147483648,"RetryFlag":-2147483648,"IntroductoryFlag":-2147483648,"TrialFlag":-2147483648,"CancelTime":-2147483648,"CancelReason":-2147483648,"AppInfo":null,"NotifyClosed":-2147483648,"RenewStatus":-2147483648,"PriceConsentStatus":-2147483648,"RenewPrice":-2147483648,"SubValid":false,"CancelledSubKeepDays":-2147483648,"Kind":-2147483648,"DeveloperChallenge":null,"ConsumptionState":-2147483648,"PayOr
        private void _OnBuyProductFailurePurchaseResultInfo(PurchaseResultInfo obj)
        {
            _DebugPurchaseResultInfo("_OnBuyProductFailurePurchaseResultInfo", obj);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            SharedLogger.Log($"{TAG}->OnApplicationPause: {pauseStatus}");
            if (!pauseStatus) _OnApplicationResumeImpl();
        }

        private void _OnApplicationResumeImpl()
        {
            SharedLogger.Log($"{TAG}->_OnApplicationResumeImpl");
            if (_purchasingAsyncOperation != null) 
                _applicationResumeAction = new HuaweiApplicationResumeAction(Time.realtimeSinceStartup + 10, () => _OnBuyProductFailure(60000));
            else RestorePurchases();
        }

        private readonly List<InAppPurchaseData> _consumablePurchaseRecord = new();
        private readonly List<InAppPurchaseData> _activeNonConsumables = new();
        private readonly List<InAppPurchaseData> _activeSubscriptions = new();
        private IHuaweiRestorePurchaseAsyncOperation _restoreAsyncOperation;
        public void RestorePurchases()
        {
            SharedLogger.Log($"{TAG}->RestorePurchases");
            var operation = _RestorePurchases();
            SharedLogger.Log($"{TAG}->RestorePurchases: {operation}");
        }

        private IHuaweiRestorePurchaseAsyncOperation _RestorePurchases()
        {
            SharedLogger.Log($"{TAG}->_RestorePurchases");
            if (_restoreAsyncOperation is { IsComplete: true }) _restoreAsyncOperation = null;
            if (_restoreAsyncOperation != null) return _restoreAsyncOperation;
            _restoreAsyncOperation = new HuaweiRestorePurchaseAsyncOperation();
            
            _cachedLastSubscriptionStateDict.Enable = true;
            _cachedLastSubscriptionStateDict.Add(clearAllFlag: true, _activeSubscriptions.ToArray());
            HMSIAPManager.Instance.GetAllOwnedPurchasesasList().Delete();
            
            _consumablePurchaseRecord.Delete();
            _activeNonConsumables.Delete();
            _activeSubscriptions.Delete();
            HMSIAPManager.Instance.RestorePurchaseRecords(null);
            HMSIAPManager.Instance.RestoreOwnedPurchases(null);
            
            return _restoreAsyncOperation;
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// OnObtainOwnedPurchaseRecordSuccess
        /// Call RestorePurchaseRecords once, callback _OnObtainOwnedPurchaseRecordSuccess 3 times. [1:3] 
        /// ------------------------------------------------------------------------------------------------------------
        private void _OnObtainOwnedPurchaseRecordSuccess(OwnedPurchasesResult restoredProducts)
        {
            SharedLogger.Log($"{TAG}->_OnObtainOwnedPurchaseRecordSuccess: restoredProducts.InAppPurchaseDataList.Count={restoredProducts.InAppPurchaseDataList.Count}");
            _restoreAsyncOperation.RestorePurchaseRecordsComplete();
            foreach (var item in restoredProducts.InAppPurchaseDataList)
            {
                if ((IAPProductType)item.Kind == IAPProductType.Consumable)
                {
                    _DebugInAppPurchaseData("RestorePurchases.Consumable: ", item);
                    _consumablePurchaseRecord.Add(item);
                }
            }
        }

        private void _OnObtainOwnedPurchaseRecordFailure(HMSException exception)
        {
            SharedLogger.Log($"{TAG}->_OnObtainOwnedPurchaseRecordFailure");
            _restoreAsyncOperation.RestorePurchaseRecordsFailure();
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// OnObtainOwnedPurchasesSuccess
        /// Call RestorePurchaseRecords once, callback _OnObtainOwnedPurchasesSuccess 3 times. [1:3] 
        /// ------------------------------------------------------------------------------------------------------------
        private void _OnObtainOwnedPurchasesSuccess(OwnedPurchasesResult restoredProducts)
        {
            SharedLogger.Log($"{TAG}->_OnObtainOwnedPurchasesSuccess: restoredProducts.InAppPurchaseDataList.Count={restoredProducts.InAppPurchaseDataList.Count}");
            _restoreAsyncOperation.RestoreOwnedPurchasesComplete();
            foreach (var item in restoredProducts.InAppPurchaseDataList)
            {
                if ((IAPProductType)item.Kind == IAPProductType.Subscription)
                {
                    _DebugInAppPurchaseData("_OnObtainOwnedPurchasesSuccess: ", item);
                    _activeSubscriptions.Add(item);
                } else if ((IAPProductType)item.Kind == IAPProductType.NonConsumable)
                {
                    _DebugInAppPurchaseData("_OnObtainOwnedPurchasesSuccess: ", item);
                    _activeNonConsumables.Add(item);
                }
            }
            if (_cachedLastSubscriptionStateDict != null) _cachedLastSubscriptionStateDict.Enable = false;
        }

        private void _OnObtainOwnedPurchasesFailure(HMSException exception)
        {
            SharedLogger.Log($"{TAG}->_OnObtainOwnedPurchasesFailure");
            _restoreAsyncOperation.RestoreOwnedPurchasesFailure();
        }

        public bool IsSubscriptionId(string id) => Config.ValidateSubscriptionsProduct(id);

        public bool ValidateSubscription(string id)
        {
            // SharedLogger.Log($"{TAG}->ValidateSubscription: {id}");
            // var p = HMSIAPManager.Instance.GetProductInfo(id);
            // _DebugProductInfo("ValidateSubscription", p);
            var errorMessage = string.Empty;
            if (!IsInitialized) errorMessage = "!IsInitialized";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.LogError($"{TAG}->ValidateSubscription: {id} Error: {errorMessage}");
                return false;
            }

            return _cachedLastSubscriptionStateDict is { Enable: true } ? _cachedLastSubscriptionStateDict.ValidateSubscription(id) : HMSIAPManager.Instance.isUserOwnThisProduct(id); // p != null && p.Status == 1;
        }

        public void DebugSubscriptionInfo(string id)
        {
            SharedLogger.Log($"{TAG}->DebugSubscriptionInfo: {id}");
            var p = HMSIAPManager.Instance.GetProductInfo(id);
            _DebugProductInfo("DebugSubscriptionInfo", p);
        }

        public IIapProduct GetProduct(string id)
        {
            SharedLogger.Log($"{TAG}->GetProduct: {id}");
            var p = HMSIAPManager.Instance.GetProductInfo(id);
            _DebugProductInfo("GetProduct", p);
            return p == null ? null : new HuaweiIapProduct(p);
        }

        public void RedirectingToSubscriptionManagementScreen(string productId)
        {
            SharedLogger.Log($"{TAG}->RedirectingToSubscriptionManagementScreen");
            HMSIAPManager.Instance.RedirectingtoSubscriptionManagementScreen();
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // Utils
        // -------------------------------------------------------------------------------------------------------------
        [Conditional("LOG_INFO")]
        private void _DebugProductInfo(string fromFunction, ProductInfo productInfo)
        {
            if (productInfo == null)
            {
                SharedLogger.Log($"{TAG}->{fromFunction}: NULL");
                return;
            }
            Dictionary<string, object> logDict = new()
            {
                {"ProductId", productInfo.ProductId},
                {"PriceType", productInfo.PriceType.ToString()},
                {"Price", productInfo.Price},
                {"MicrosPrice", productInfo.MicrosPrice},
                {"OriginalLocalPrice", productInfo.OriginalLocalPrice},
                {"OriginalMicroPrice", productInfo.OriginalMicroPrice},
                {"Currency", productInfo.Currency},
                {"ProductName", productInfo.ProductName},
                {"ProductDesc", productInfo.ProductDesc},
                {"SubPeriod", productInfo.SubPeriod},
                {"SubSpecialPrice", productInfo.SubSpecialPrice},
                {"SubSpecialPriceMicros", productInfo.SubSpecialPriceMicros},
                {"SubSpecialPeriod", productInfo.SubSpecialPeriod},
                {"SubSpecialPeriodCycles", productInfo.SubSpecialPeriodCycles},
                {"SubFreeTrialPeriod", productInfo.SubFreeTrialPeriod},
                {"SubGroupId", productInfo.SubGroupId},
                {"SubGroupTitle", productInfo.SubGroupTitle},
                {"SubProductLevel", productInfo.SubProductLevel},
                {"Status", productInfo.Status},
                {"OfferUsedStatus", productInfo.OfferUsedStatus}
            };
            SharedLogger.Log($"{TAG}->{fromFunction}: {JsonConvert.SerializeObject(logDict)}");
        }
        
        [Conditional("LOG_INFO")]
        private void _DebugPurchaseResultInfo(string fromFunction, PurchaseResultInfo productInfo)
        {
            Dictionary<string, object> logDict = new()
            {
                {"ReturnCode", productInfo.ReturnCode},
                {"ErrMsg", productInfo.ErrMsg},
                {"InAppPurchaseData", _ConvertInAppPurchaseData(productInfo.InAppPurchaseData)},
                {"InAppPurchaseDataRawJSON", productInfo.InAppPurchaseDataRawJSON},
                {"InAppDataSignature", productInfo.InAppDataSignature},
            };
            SharedLogger.Log($"{TAG}->{fromFunction}: {JsonConvert.SerializeObject(logDict)}");
        }
        
        [Conditional("LOG_INFO")]
        private void _DebugInAppPurchaseData(string fromFunction, InAppPurchaseData entity)
        {
            var convertedData = _ConvertInAppPurchaseData(entity);
            SharedLogger.Log($"{TAG}->{fromFunction}: {JsonConvert.SerializeObject(convertedData)}");
        }

        private Dictionary<string, object> _ConvertInAppPurchaseData(InAppPurchaseData entity)
        {
            return new Dictionary<string, object>()
            {
                {"ApplicationId", entity.ApplicationId},
                {"AutoRenewing", entity.AutoRenewing},
                {"OrderID", entity.OrderID},
                {"PackageName", entity.PackageName},
                {"ProductId", entity.ProductId},
                {"ProductName", entity.ProductName},
                {"PurchaseTime", entity.PurchaseTime},
                {"PurchaseState", entity.PurchaseState},
                {"DeveloperPayload", entity.DeveloperPayload},
                {"PurchaseToken", entity.PurchaseToken},
                {"PurchaseType", entity.PurchaseType},
                {"Currency", entity.Currency},
                {"Price", entity.Price},
                {"Country", entity.Country},
                {"LastOrderId", entity.LastOrderId},
                {"ProductGroup", entity.ProductGroup},
                {"OriPurchaseTime", entity.OriPurchaseTime},
                {"SubscriptionId", entity.SubscriptionId},
                {"Quantity", entity.Quantity},
                {"DaysLasted", entity.DaysLasted},
                {"NumOfPeriods", entity.NumOfPeriods},
                {"NumOfDiscount", entity.NumOfDiscount},
                {"ExpirationDate", entity.ExpirationDate},
                {"ExpirationIntent", entity.ExpirationIntent},
                {"RetryFlag", entity.RetryFlag},
                {"IntroductoryFlag", entity.IntroductoryFlag},
                {"TrialFlag", entity.TrialFlag},
                {"CancelTime", entity.CancelTime},
                {"CancelReason", entity.CancelReason},
                {"AppInfo", entity.AppInfo},
                {"NotifyClosed",entity.NotifyClosed},
                {"RenewStatus", entity.RenewStatus},
                {"PriceConsentStatus", entity.PriceConsentStatus},
                {"RenewPrice", entity.RenewPrice},
                {"SubValid", entity.SubValid},
                {"CancelledSubKeepDays", entity.CancelledSubKeepDays},
                {"Kind", entity.Kind},
                {"DeveloperChallenge", entity.DeveloperChallenge},
                {"ConsumptionState", entity.ConsumptionState},
                {"PayOrderId", entity.PayOrderId},
                {"PayType", entity.PayType},
                {"DeferFlag", entity.DeferFlag},
                {"OriSubscriptionId", entity.OriSubscriptionId},
                {"CancelWay", entity.CancelWay},
                {"CancellationTime", entity.CancellationTime},
                {"ResumeTime", entity.ResumeTime},
                {"GraceExpirationTime", entity.GraceExpirationTime},
                {"AccountFlag", entity.AccountFlag}
            };
        }
    }
}
#endif