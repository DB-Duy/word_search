using System.Collections.Generic;

namespace Shared
{
    public static class SharedLogTag
    {
        public const string UIBind = "[UIBind]";
        public const string Ioc = "Ioc";
        public const string Tutorial = "Tutorial";
        public const string Loading = "Loading";
        public const string RemoteConfig = "[RemoteConfig]";
        public const string Version = "version";
        public const string Iap = "[iap]";
        public const string ParentControl = "[ParentControl]";
        public const string ParentControlNAppLovin = "[ParentControl,AppLovin]";
        public const string ParentControlNAdverty = "[ParentControl,Adverty]";
        public const string ParentControlNBidMachine = "[ParentControl,BidMachine]";
        public const string ParentControlNGadsme = "[ParentControl,Gadsme]";
        public const string ParentControlNHyBid = "[ParentControl,HyBid]";
        public const string ParentControlNInMobi = "[ParentControl,InMobi]";
        public const string ParentControlNInneractiveAdOrFyber = "[ParentControl,InneractiveAd,Fyber]";
        public const string ParentControlNLevelPlay = "[ParentControl,LevelPlay]";
        public const string ParentControlNSmaato = "[ParentControl,Smaato]";
        public const string ParentControlNMobileFuse = "[ParentControl,MobileFuse]";
        public const string Ump = "[Ump]";
        public const string UmpNFyber = "[Ump,Fyber]";
        public const string UmpNOdeeo = "[Ump,Odeeo]";
        public const string UmpNLevelPlay = "[Ump,LevelPlay]";
        public const string UmpNInMobi = "[Ump,InMobi]";
        public const string UmpNFirebase = "[Ump,Firebase]";
        public const string UmpNAudioMob = "[Ump,AudioMob]";
        public const string UmpNAdverty = "[Ump,Adverty]";
        public const string Adjust = "[Adjust]";
        public const string AdjustNAppMetrica = "[Adjust,AppMetrica]";
        public const string AdjustNReferrerNXiaomi = "[Adjust,Referrer,Xiaomi]";
        public const string AdjustNReferrerNSamsung = "[Adjust,Referrer,Samsung]";
        public const string AdjustNReferrerNVivo = "[Adjust,Referrer,Vivo]";
        public const string Bright = "[Bright]";
        
        
        public const string AdjustNFirebase = "[Adjust,Firebase]";
        public static readonly List<string> UmpNAdjust = new() { Ump, Adjust };
        public static readonly string UmpNAdjust_ = "[Ump,Adjust]";
        public const string AdQuality = "AdQuality";
        public const string LevelPlay = "LevelPlay";
        public const string UserPropertyNLevelPlay = "[UserProperty,LevelPlay]";
        public const string AdQualityNLevelPlay = "[AdQuality,LevelPlay]";
        public const string AppMetrica = "[AppMetrica]";
        public const string UserPropertyNAppMetrica = "[UserProperty,AppMetrica]";
        public const string Facebook = "[Facebook]";
        public const string Fyber = "Fyber";
        public static readonly List<string> FyberNUmp = new() { Fyber, Ump };
        public const string Firebase = "Firebase";
        public const string FirebaseNMessaging = "[Firebase,Messaging]";
        public const string FirebaseNMessagingNAppsFlyer = "[Firebase,Messaging,AppsFlyer]";
        public const string FirebaseNS2S = "[Firebase,S2S]";
        public const string UserPropertyNFirebase = "[UserProperty,Firebase]";
        public const string FirebaseNConfig = "[Firebase,RemoteConfig]";
        public const string Tracking = "[Tracking]";
        public const string TrackingNAppMetrica = "[Tracking,AppMetrica]";
        public const string TrackingNFirebase = "[Tracking,Firebase]";
        public const string TrackingNS2S = "[Tracking,S2S]";
        public const string TrackingNInterrupt = "[Tracking,Interrupt]";
        public const string TrackingNMmp = "[Tracking,Mmp]";
        public const string TrackingNMmpNAdjust = "[Tracking,Mmp,Adjust]";
        public const string TrackingNMmpNAppsFlyer = "[Tracking,Mmp,AppsFlyer]";
        public static readonly List<string> TrackingNAdjustNIap = new() { Tracking, Adjust, Iap };
        public static readonly string TrackingNAdjustNIap_ = "[Tracking,Adjust,Iap]";
        public static readonly List<string> TrackingNAdjustNAds = new() { Tracking, Adjust, "Ad" };
        public static readonly string TrackingNAdjustNAds_ = "[Tracking,Adjust,Ad]";
        public static readonly string TrackingNAppsFlyerNAds_ = "[Tracking,AppsFlyer,Ad]";
        
        public const string AppsFlyer = "[AppsFlyer]";
        public const string AppsFlyerNAppMetrica = "[AppsFlyer,AppMetrica]";
        public const string Odeeo = "Odeeo";
        public const string AudioMob = "AudioMob";
        public const string Gadsme = "[Gadsme]";
        public const string AudioAds = "[AudioAds]";
        public const string AudioAdsNOdeoo = "[AudioAds,Odeeo]";
        public const string AudioAdsNAudioMob = "[AudioAds,AudioMob]";
        public const string AudioAdsNGadsme = "[AudioAds,Gadsme]";
        public const string InPlayAds = "[InPlayAds]";
        public const string InPlayAdsNGadsme = "[InPlayAds,Gadsme]";
        public const string Adverty = "Adverty";
        public static readonly string InPlayAdsNAdverty = "[InPlayAds,Adverty]";
        public const string InAppUpdate = "[InAppUpdate]";
        public const string StoreRating = "StoreRating";
        public const string Privacy = "Privacy";
        public const string Repository = "Repository";
        public static readonly List<string> RemoteConfigNInRepository = new() { RemoteConfig, Repository };
        public const string Session = "Session";
        public static readonly List<string> SessionNRemoteConfig = new() { Session, RemoteConfig };
        public const string S2S = "S2S";
        public static readonly List<string> S2SNIap = new() { S2S, Iap };
        public static readonly string S2SNIap_ = "[S2S,Iap]";
        public static readonly List<string> AdNLevelPlay = new() { "Ad", "LevelPlay" };
        public static readonly string AdNLevelPlay_ = "[Ad,LevelPlay]";
        public static readonly string AdNLevelPlayNMoloco = "[Ad,LevelPlay,Moloco]";
        public static readonly string AdNLevelPlayNAps = "[Ad,LevelPlay,Aps]";
        public static readonly string AdNLevelPlayNUmp = "[Ad,LevelPlay,Ump]";
        public static readonly List<string> AdNLevelPlayNBanner = new() { "Ad", "LevelPlay", "Banner" };
        public static readonly string AdNLevelPlayNBanner_ = "[Ad,LevelPlay,Banner]";
        public static readonly string AdNLevelPlayNMrec = "[Ad,LevelPlay,Mrec]";
        public static readonly string AdNBanner = "[Ad,Banner]";
        public static readonly List<string> AdNLevelPlayNBannerNAps = new() { "Ad", "LevelPlay", "Banner", "Aps" };
        public static readonly string AdNLevelPlayNBannerNAps_ = "[Ad,LevelPlay,Banner,Aps]";
        public static readonly string AdNLevelPlayNMrecNAps = "[Ad,LevelPlay,Mrec,Aps]";
        public static readonly List<string> AdNLevelPlayNInterstitial = new() { "Ad", "LevelPlay", "Interstitial"};
        public static readonly string AdNLevelPlayNInterstitial_ = "[Ad,LevelPlay,Interstitial]";
        public static readonly string AdNInterstitial = "[Ad,Interstitial]";
        public const string AdNLevelPlayNInterstitialNAps = "[Ad,LevelPlay,Interstitial,Aps]";
        public static readonly List<string> AdNLevelPlayNReward = new() { "Ad", "LevelPlay", "Reward" };
        public static readonly string AdNLevelPlayNReward_ = "[Ad,LevelPlay,Reward]";
        public static readonly List<string> AdNLevelPlayNRewardNAps = new() { "Ad", "LevelPlay", "Reward", "Aps" };
        public static readonly string Audio = "[Audio]";
        public const string TrackingAuthorization = "[TrackingAuthorization]";
        public const string TrackingAuthorizationNFan = "[TrackingAuthorization,Fan]";
        
        public const string AdNLevelPlayNBigo = "[Ad,LevelPlay,Bigo]";
        public const string CheatNTime = "[Cheat,Time]";
        
    }
}