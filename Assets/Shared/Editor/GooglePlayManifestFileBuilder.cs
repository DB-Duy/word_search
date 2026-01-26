using SharedEditor.Editor.Distribution.SharedFirebase;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Shared.Editor
{
    [InitializeOnLoad]
    public class GooglePlayManifestFileBuilder
    {
        static GooglePlayManifestFileBuilder()
        {
            EditorApplication.delayCall += OnAfterDomainReload;
        }

        private static void OnAfterDomainReload()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }
            
            if (!SharedEditorUtils.IsBuildConfigFileExisted())
            {
                Debug.LogError("build_config.json file not found. Skipping GooglePlayManifestFileBuilder Domain reload.");
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }

            if (!SharedEditorUtils.IsKeyConfigFileExisted())
            {
                Debug.LogError("Config.json file not found. Skipping GooglePlayManifestFileBuilder Domain reload.");
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }
            
            if (!SharedEditorUtils.IsGoogleServicesFileExisted())
            {
                Debug.LogError("google-services.json file not found. Skipping GooglePlayManifestFileBuilder Domain reload.");
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }

            var (bundleId, facebookAppId, facebookClientToken, admobAppId) = SharedEditorUtils.GetKeyConfigValues("android", "bundle_id", "facebook_id", "facebook_client_token", "admob_app_id");
            var googleServices = GoogleServices.NewInstance();
            var clientInfo = googleServices.GetByPackageName(bundleId);
            var apikey = clientInfo.GetFirstApiKey();

            var ympFirebaseDefaultAppId = clientInfo.ClientInfo.MobileSdkAppId;
            var ympGcmDefaultSenderId = googleServices.ProjectInfo.ProjectNumber;
            var ympFirebaseDefaultApiKey = apikey == null ? "" : apikey.CurrentKey;
            var ympFirebaseDefaultProjectId = googleServices.ProjectInfo.ProjectId;

            var xmlLines = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"" package=""{bundleId}"" xmlns:tools=""http://schemas.android.com/tools"">
  <application android:name=""com.unity3d.player.MyApplication"" android:gwpAsanMode=""always"" android:hardwareAccelerated=""true"" tools:replace=""android:enableOnBackInvokedCallback"" android:enableOnBackInvokedCallback=""true"">
    <activity android:name=""com.unity3d.player.UnityPlayerActivityWithANRWatchDog"" android:theme=""@style/UnityThemeSelector"">
      <intent-filter>
        <action android:name=""android.intent.action.MAIN"" />
        <category android:name=""android.intent.category.LAUNCHER"" />
      </intent-filter>
      <meta-data android:name=""unityplayer.UnityActivity"" android:value=""true"" />
    </activity>
    <service android:name=""com.google.firebase.messaging.cpp.ListenerService"" android:exported=""true"" android:enabled=""false"" tools:node=""replace"" />
    <service android:name=""com.google.firebase.messaging.MessageForwardingService"" android:exported=""true"" />
    <!-- <receiver android:name=""com.adjust.sdk.AdjustReferrerReceiver"" android:permission=""android.permission.INSTALL_PACKAGES"" android:exported=""true"">
      <intent-filter>
        <action android:name=""com.android.vending.INSTALL_REFERRER"" />
      </intent-filter>
    </receiver> -->
    <activity android:name=""com.amazon.device.ads.DTBInterstitialActivity"" />
    <activity android:name=""com.amazon.device.ads.DTBAdActivity"" />
    <activity android:name=""com.facebook.unity.FBUnityLoginActivity"" android:configChanges=""fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen"" android:theme=""@android:style/Theme.Translucent.NoTitleBar.Fullscreen"" />
    <activity android:name=""com.facebook.unity.FBUnityDialogsActivity"" android:configChanges=""fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen"" android:theme=""@android:style/Theme.Translucent.NoTitleBar.Fullscreen"" />
    <activity android:name=""com.facebook.unity.FBUnityGamingServicesFriendFinderActivity"" android:configChanges=""fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen"" android:theme=""@android:style/Theme.Translucent.NoTitleBar.Fullscreen"" />
    <activity android:name=""com.facebook.unity.FBUnityAppLinkActivity"" android:exported=""true"" />
    <activity android:name=""com.facebook.unity.FBUnityDeepLinkingActivity"" android:exported=""true"" />
    <activity android:name=""com.facebook.unity.FBUnityGameRequestActivity"" />
    <activity android:name=""com.my.target.common.MyTargetActivity"" android:configChanges=""keyboard|keyboardHidden|orientation|screenLayout|uiMode|screenSize|smallestScreenSize""/>
    <meta-data android:name=""com.facebook.sdk.ApplicationId"" android:value=""fb{facebookAppId}"" />
    <meta-data android:name=""com.facebook.sdk.ClientToken"" android:value=""{facebookClientToken}"" />
    <meta-data android:name=""com.facebook.sdk.AutoLogAppEventsEnabled"" android:value=""true"" />
    <meta-data android:name=""com.facebook.sdk.AdvertiserIDCollectionEnabled"" android:value=""true"" />
    <meta-data android:name=""com.google.android.gms.ads.flag.OPTIMIZE_INITIALIZATION"" android:value=""true"" />
    <meta-data android:name=""com.google.android.gms.ads.flag.OPTIMIZE_AD_LOADING"" android:value=""true"" />
    <meta-data android:name=""com.google.android.gms.ads.APPLICATION_ID"" android:value=""{admobAppId}"" />
    <!--AppMetrica - Your Firebase identificators -->
    <meta-data android:name=""ymp_firebase_default_app_id"" android:value=""{ympFirebaseDefaultAppId}"" />
    <meta-data android:name=""ymp_gcm_default_sender_id"" android:value=""number:{ympGcmDefaultSenderId}"" />
    <meta-data android:name=""ymp_firebase_default_api_key"" android:value=""{ympFirebaseDefaultApiKey}"" />
    <meta-data android:name=""ymp_firebase_default_project_id"" android:value=""{ympFirebaseDefaultProjectId}"" />
    <meta-data android:name=""google_analytics_default_allow_analytics_storage"" android:value=""true"" />
    <meta-data android:name=""google_analytics_default_allow_ad_storage"" android:value=""true"" />
    <meta-data android:name=""google_analytics_default_allow_ad_user_data"" android:value=""true"" />
    <meta-data android:name=""google_analytics_default_allow_ad_personalization_signals"" android:value=""true"" />
    <meta-data android:name=""firebase_performance_logcat_enabled"" android:value=""true"" />
    <provider android:name=""com.facebook.FacebookContentProvider"" android:authorities=""com.facebook.app.FacebookContentProvider{facebookAppId}"" android:exported=""true"" />
    <!-- AnalyticsFixPropertyRemover -->
  </application>
  <uses-permission android:name=""android.permission.WAKE_LOCK"" />
  <uses-permission android:name=""com.google.android.gms.permission.AD_ID"" />
  <uses-permission android:name=""android.permission.INTERNET"" />
  <uses-permission android:name=""android.permission.ACCESS_WIFI_STATE"" />
  <uses-permission android:name=""android.permission.ACCESS_NETWORK_STATE"" />
  <uses-permission android:name=""android.permission.VIBRATE"" />
  <queries><package android:name=""com.android.chrome"" /></queries>
  <uses-permission android:name=""com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE"" />
  <uses-permission android:name=""android.permission.ACCESS_COARSE_LOCATION"" tools:node=""remove""/>
  <uses-permission android:name=""android.permission.ACCESS_FINE_LOCATION"" tools:node=""remove""/>
  <uses-permission android:name=""android.permission.CAMERA"" tools:node=""remove""/>
  <uses-permission android:name=""android.permission.RECORD_AUDIO"" tools:node=""remove""/>
  <uses-permission android:name=""com.samsung.android.mapsagent.permission.READ_APP_INFO"" tools:node=""remove""/>
  <uses-permission android:name=""com.huawei.appmarket.service.commondata.permission.GET_COMMON_DATA"" tools:node=""remove""/>
  <uses-permission android:name=""android.permission.WRITE_EXTERNAL_STORAGE"" tools:node=""remove"" />
  <uses-permission android:name=""android.permission.READ_PHONE_STATE"" tools:node=""remove"" />
  <uses-permission android:name=""android.permission.READ_EXTERNAL_STORAGE"" tools:node=""remove"" />
</manifest>";
            _ = SharedEditorUtils.WriteProjectRelativeFile("Assets/Plugins/Android/AndroidManifest.xml", xmlLines, silent: true);
            EditorApplication.delayCall -= OnAfterDomainReload;
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ GooglePlayManifestFileBuilder Domain reload complete!");
#endif
        }
    }
}