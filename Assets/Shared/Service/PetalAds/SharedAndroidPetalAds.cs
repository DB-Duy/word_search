#if HUAWEI && UNITY_ANDROID
using UnityEngine;

namespace Shared.SharedPetalAds
{
    public class SharedAndroidPetalAds : ISharedPetalAds
    {
        public string Consent => _hwAdsClass.CallStatic<AndroidJavaObject>("getRequestOptions").Call<string>("getConsent");
        public string SdkVersion => _hwAdsClass.CallStatic<string>("getSDKVersion");

        private readonly AndroidJavaClass _hwAdsClass = new("com.huawei.hms.ads.HwAds");
    }
}
#endif