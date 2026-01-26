#if APPS_FLYER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AppsFlyerSDK;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Service.TrackingAuthorization;
using Shared.Service.Ump;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.SharedAppsFlyer
{
    /// <summary>
    /// AppsFlyerObject.prefab
    /// </summary>
    [DisallowMultipleComponent]
    public class AppsFlyerObjectScript : IoCMonoBehavior, IAppsFlyerConversionData, ISharedUtility, IAppsFlyerPurchaseValidation
    {
        [Inject] IConfig _config;

        private void Start()
        {
            // https://dev.appsflyer.com/hc/docs/basicintegration#request-listeners-optional
            AppsFlyer.OnRequestResponse += _OnRequestResponse;
            // https://dev.appsflyer.com/hc/docs/basicintegration#request-listeners-optional
            AppsFlyer.OnInAppResponse += _OnInAppResponse;

            // These fields are set from the editor so do not modify!
            //******************************//
            AppsFlyer.setIsDebug(SharedSymbols.IsDevelopment);
            
            // https://dev.appsflyer.com/hc/docs/basicintegration#collect-idfa-with-attrackingmanager
#if UNITY_IOS && !UNITY_EDITOR
            // Dont show ATT at this step.
            //AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
#endif
#if UNITY_WSA_10_0 && !UNITY_EDITOR
                AppsFlyer.initSDK(devKey, UWPAppID, getConversionData ? this : null);
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
            AppsFlyer.initSDK(devKey, macOSAppID, getConversionData ? this : null);
#elif APPSTORE
            this.LogInfo(SharedLogTag.AppsFlyer, "call", $"AppsFlyer.initSDK({_config.AppsFlyerDevKey}, {_config.AppStoreId}, this);");
            _ThrowIfAppStoreIdIsNullOrEmpty(_config.AppStoreId);
            AppsFlyer.initSDK(_config.AppsFlyerDevKey, _config.AppStoreId ?? "", this);
#else
            AppsFlyer.initSDK(_config.AppsFlyerDevKey, "", this);
#endif
            //******************************/
#if (GOOGLE_PLAY || APPSTORE) && IAP
            _InitPurchaseConnector();
#endif
            this.LogInfo(SharedLogTag.AppsFlyer, "AppsFlyer.getSdkVersion()", AppsFlyer.getSdkVersion(), "pluginVersion", AppsFlyer.kAppsFlyerPluginVersion);
            // https://dev.appsflyer.com/hc/docs/basicintegration#set-customer-user-id
            AppsFlyer.setCustomerUserId(SystemInfo.deviceUniqueIdentifier);
            this.LogInfo(SharedLogTag.AppsFlyer, "call", $"AppsFlyer.setCustomerUserId({SystemInfo.deviceUniqueIdentifier})");
            // https://dev.appsflyer.com/hc/docs/basicintegration#send-consent-for-dma-compliance
            AppsFlyer.enableTCFDataCollection(true);
            this.LogInfo(SharedLogTag.AppsFlyer, "call", "AppsFlyer.enableTCFDataCollection(true);");
            
            StartCoroutine(_WaitAndStart());
        }

        private IEnumerator _WaitAndStart()
        {
#if USING_UMP
            while (!UmpFlag.IsUmpReady) yield return null;
#endif
#if UNITY_IOS && !UNITY_EDITOR
            while (!AttFlag.IsInitialized) yield return null;
#endif
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(_WaitAndStart));
            AppsFlyer.startSDK();
            yield return null;
        }

#if GOOGLE_PLAY || APPSTORE
        private void _InitPurchaseConnector()
        {
            // Purchase connector implementation 
            AppsFlyerPurchaseConnector.init(this, Store.GOOGLE);
            AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
            AppsFlyerPurchaseConnector.setIsSandbox(SharedSymbols.IsDevelopment);
            AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions, AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
            AppsFlyerPurchaseConnector.build();
            AppsFlyerPurchaseConnector.startObservingTransactions();
        }
#endif

        /// <summary>
        /// [AppsFlyer] AppsFlyerObjectScript->LogInfo: {"f":"requestResponseReceived","response":"{\"statusCode\":42}"}
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public void requestResponseReceived(string response)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", "requestResponseReceived", nameof(response), response);
        }
        
        // This is the missing callback method
        // ReSharper disable once InconsistentNaming
        public void inAppResponseReceived(string response)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", "inAppResponseReceived", nameof(response), response);
        }
        
        private void _OnRequestResponse(object sender, EventArgs e)
        {
            if (e is AppsFlyerRequestEventArgs args)
            {
                this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(_OnRequestResponse), nameof(args.statusCode), args.statusCode);
                return;
            }
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(_OnRequestResponse), "type", e?.GetType().Name);
        }


        // private readonly Dictionary<int, string> ResponseSDescriptions = new()
        // {
        //     { 200, "null"},
        //     { 10, "Event timeout. Check 'minTimeBetweenSessions' param"},
        //     { 11, "Skipping event because 'isStopTracking' enabled" },
        //     { 40, "Network error: Error description comes from Android" },
        //     { 41, "No dev key" },
        //     { 50, "\"Status code failure\" + actual response code from the server"}
        // };
        private void _OnInAppResponse(object sender, EventArgs args)
        {
            if (args is AppsFlyerRequestEventArgs afArgs)
            {
                this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(_OnInAppResponse), nameof(afArgs.statusCode), afArgs.statusCode, nameof(afArgs.errorDescription), afArgs.errorDescription);
                return;
            }

            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(_OnInAppResponse), "type", args?.GetType().Name);
        }


        // void Update()
        // {
        //
        // }

        // https://dev.appsflyer.com/hc/docs/conversion-data-unity
        // Mark AppsFlyer CallBacks
        public void onConversionDataSuccess(string conversionData)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(onConversionDataSuccess), nameof(conversionData), conversionData);
#if APPMETRICA
            Io.AppMetrica.AppMetrica.ReportExternalAttribution(Io.AppMetrica.ExternalAttributions.AppsFlyer(conversionData));
#endif
        }

        // https://dev.appsflyer.com/hc/docs/conversion-data-unity
        public void onConversionDataFail(string error)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(onConversionDataFail), nameof(error), error);
        }

        // https://dev.appsflyer.com/hc/docs/conversion-data-unity
        public void onAppOpenAttribution(string attributionData)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(onAppOpenAttribution), nameof(attributionData), attributionData);
            // add direct deeplink logic here
        }

        // https://dev.appsflyer.com/hc/docs/conversion-data-unity
        public void onAppOpenAttributionFailure(string error)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(onAppOpenAttributionFailure), nameof(error), error);
        }

        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(didReceivePurchaseRevenueValidationInfo), nameof(validationInfo), validationInfo);
        }

        public void didReceivePurchaseRevenueError(string error)
        {
            this.LogInfo(SharedLogTag.AppsFlyer, "f", nameof(didReceivePurchaseRevenueError), nameof(error), error);
        }

        [Conditional("LOG_INFO")]
        private void _ThrowIfAppStoreIdIsNullOrEmpty(string appStoreId)
        {
            if (string.IsNullOrEmpty(appStoreId))
            {
                throw new ArgumentNullException("AppsFlyerObjectScript " + nameof(appStoreId));
            }
        }
    }
}
#endif