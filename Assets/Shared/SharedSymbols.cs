using System.Collections.Generic;

namespace Shared
{
    public static class SharedSymbols
    {
#if LOG_INFO
        public static readonly bool IsDevelopment = true;
#else
        public static readonly bool IsDevelopment = false;
#endif

#if APPSTORE
        public static readonly bool AppStoreEnable = true;
#else
        public static readonly bool AppStoreEnable = false;
#endif

#if APPSTORE_CHINA || USING_CHINA
        public static readonly bool AppStoreChinaEnable = true;
#else
        public static readonly bool AppStoreChinaEnable = false;
#endif

#if GOOGLE_PLAY
        public static readonly bool GooglePlayEnable = true;
#else
        public static readonly bool GooglePlayEnable = false;
#endif

#if HUAWEI
        public static readonly bool HuaweiEnable = true;
#else
        public static readonly bool HuaweiEnable = false;
#endif

#if FACEBOOK_INSTANT
        public static readonly bool FacebookInstantEnable = true;
#else
        public static readonly bool FacebookInstantEnable = false;
#endif

#if LEVEL_PLAY
        public static readonly bool LevelPlayEnable = true;
#else
        public static readonly bool LevelPlayEnable = false;
#endif

#if APS
        public static readonly bool ApsEnable = true;
#else
        public static readonly bool ApsEnable = false;
#endif

#if TOPON
            public static readonly bool TopOnEnable = true;
#else
        public static readonly bool TopOnEnable = false;
#endif


#if AUDIO_MOB
        public static readonly bool AudioMobEnable = true;
#else
        public static readonly bool AudioMobEnable = false;
#endif

#if ODEEO_AUDIO
        public static readonly bool AudioOdeeoEnable = true;
#else
        public static readonly bool AudioOdeeoEnable = false;
#endif

#if GADSME_AUDIO
        public static readonly bool AudioGadsmeEnable = true;
#else
            public static readonly bool AudioGadsmeEnable = false;
#endif

#if GADSME
        public static readonly bool InPlayGadsmeEnable = true;
#else
            public static readonly bool InPlayGadsmeEnable = false;
#endif

#if GADSME
        public static readonly bool InPlayAdvertyEnable = true;
#else
            public static readonly bool InPlayAdvertyEnable = false;
#endif

#if USING_UMP
        public static readonly bool UmpEnable = true;
#else
            public static readonly bool UmpEnable = false;
#endif

#if FIREBASE
        public static readonly bool FirebaseEnable = true;
#else
            public static readonly bool FirebaseEnable = false;
#endif

#if FIREBASE_WEBGL
            public static readonly bool FirebaseWebEnable = true;
#else
        public static readonly bool FirebaseWebEnable = false;
#endif

#if DEV_TO_DEV
        public static readonly bool DevToDevEnable = true;
#else
            public static readonly bool DevToDevEnable = false;
#endif

#if ADJUST
        public static readonly bool AdjustEnable = true;
#else
            public static readonly bool AdjustEnable = false;
#endif

#if APPMETRICA
        public static readonly bool AppMetricaEnable = true;
#else
            public static readonly bool AppMetricaEnable = false;
#endif

#if AUDIO_ADS
        public static readonly bool AudioAdEnable = true;
#else
            public static readonly bool AudioAdEnable = false;
#endif

#if FACEBOOK_SDK
        public static readonly bool FacebookSdkEnable = true;
#else
            public static readonly bool FacebookSdkEnable = false;
#endif

#if FAN
        public static readonly bool FanEnable = true;
#else
            public static readonly bool FanEnable = false;
#endif

#if IN_APP_UPDATE
            public static readonly bool InAppUpdateEnable = true;
#else
        public static readonly bool InAppUpdateEnable = false;
#endif

#if BRIGHT_DATA_SDK
        public static readonly bool BrightDataEnable = true;
#else
            public static readonly bool BrightDataEnable = false;
#endif

#if BIGO
        public static readonly bool BigoEnable = true;
#else
            public static readonly bool BigoEnable = false;
#endif

#if ODEEO_AUDIO
        public static readonly bool OdeeoEnable = true;
#else
            public static readonly bool OdeeoEnable = false;
#endif

#if GADSME_AUDIO
        public static readonly bool GadsmeAudioEnable = true;
#else
            public static readonly bool GadsmeAudioEnable = false;
#endif

#if ADVERTY
        public static readonly bool AdvertyEnable = true;
#else
            public static readonly bool AdvertyEnable = false;
#endif

#if PREMIUM_DIALOG
        public static readonly bool PremiumDialogEnable = true;
#else
            public static readonly bool PremiumDialogEnable = false;
#endif

#if USER_RATING
        public static readonly bool UserRatingEnable = true;
#else
            public static readonly bool UserRatingEnable = false;
#endif

#if INPLAY_ADS
        public static readonly bool InPlayAdEnable = true;
#else
            public static readonly bool InPlayAdEnable = false;
#endif

#if APP_SHARE
        public static readonly bool AppShareEnable = true;
#else
        public static readonly bool AppShareEnable = false;
#endif

#if PUBMATIC
        public static readonly bool PubmaticEnable = true;
#else
        public static readonly bool PubmaticEnable = false;
#endif

        public static Dictionary<string, bool> Enables = new()
        {
            { "APPSTORE", AppStoreEnable },
            { "APPSTORE_CHINA", AppStoreChinaEnable },
            { "GOOGLE_PLAY", GooglePlayEnable },
            { "HUAWEI", HuaweiEnable },
            { "FACEBOOK_INSTANT", FacebookInstantEnable },
            { "LEVEL_PLAY", LevelPlayEnable },
            { "APS", ApsEnable },
            { "TOPON", TopOnEnable },
            { "USING_UMP", UmpEnable },
            { "FIREBASE", FirebaseEnable },
            { "FIREBASE_WEBGL", FirebaseWebEnable },
            { "DEV_TO_DEV", DevToDevEnable },
            { "ADJUST", AdjustEnable },
            { "APPMETRICA", AppMetricaEnable },
            { "AUDIO_ADS", AudioAdEnable },
            { "FACEBOOK_SDK", FacebookSdkEnable },
            { "BRIGHT_DATA_SDK", BrightDataEnable },
            { "FAN", FanEnable },
            { "BIGO", BigoEnable },
            { "ODEEO_AUDIO", OdeeoEnable },
            { "GADSME_AUDIO", GadsmeAudioEnable },
            { "AUDIO_MOB", AudioMobEnable },
            { "ADVERTY", AdvertyEnable },
            { "GADSME", InPlayGadsmeEnable },
            { "PREMIUM_DIALOG", PremiumDialogEnable },
            { "USER_RATING", UserRatingEnable },
            { "INPLAY_ADS", InPlayAdEnable },
            { "APP_SHARE", AppShareEnable },
            { "IN_APP_UPDATE", InAppUpdateEnable },
            { "PUBMATIC", PubmaticEnable }
        };
    }
}