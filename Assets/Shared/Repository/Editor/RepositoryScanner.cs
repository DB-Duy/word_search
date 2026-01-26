using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Shared.Core.IoC;
using Shared.Repository.Ads;
using Shared.Repository.AppMetrica;
using Shared.Repository.Audio;
using Shared.Repository.Firebase;
using Shared.Repository.GameInterrupt;
using Shared.Repository.InAppUpdate;
using Shared.Repository.InPlayAds;
using Shared.Repository.Level;
using Shared.Repository.Mmp;
using Shared.Repository.ParentControl;
using Shared.Repository.Premium;
using Shared.Repository.S2S;
using Shared.Repository.Session;
using Shared.Repository.StoreRating;
using Shared.Repository.TrackingAuthorization;
using Shared.Repository.Ump;
using Shared.Repository.Vibration;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

#if ADJUST
using Shared.Repository.Adjust;
#endif

namespace Shared.Repository.Editor
{
    [InitializeOnLoad]
    public class RepositoryScanner
    {
        private const string Tag = "RepositoryScanner";
        
        public class MyDefine
        {
            public readonly string Name;
            public readonly string DefaultValue;

            public MyDefine(string name, string defaultValue)
            {
                Name = name;
                DefaultValue = defaultValue;
            }
        }

        private static readonly List<string> IgnoredRepositories = new()
        {
            "GdprAppliesRepository", // 
            "PurposeConsentsRepository", // 
            "TcStringRepository", //
            "UsPrivacyStringRepository", // 
            "EventCountRepository",
            "IapItemRepository", //
            "SharedFeaturePrefabRepository", //
            "AudioClipRepository", //
            "CacheAudioClipRepository",//
        };

        private static readonly Dictionary<string, MyDefine> DefaultRepositoryValues = new ()
        {
            // Remote Config.
            { typeof(UmpConfigRepository).FullName!, new MyDefine("ConsentConfig", "{\"enable_consent_android\":true,\"enable_consent_iOS\":true}") },
            { typeof(StoreRatingConfigRepository).FullName!, new MyDefine("FakeRatingPopup", "{ \"show_popup\": 1, \"show_after_level\": 10 }") },
            // { typeof(SessionConfigRepository).FullName!, new MyDefine("SessionTimeoutConfig", "{\"session_timeout\":300}") },
            { typeof(S2SConfigRepository).FullName!, new MyDefine("IapVerificationConfig", "{\"unlocked\":true,\"remote_url\":\"https://s2s.indiez.net/app/iap/google-play/verify\",\"api_key\":\"aeee2cea-5b1b-452d-84e5-bb984341596e\"}")},
            { typeof(MmpConfigRepository).FullName!, new MyDefine("EventsForMMP", "[]") },
            { typeof(InPlayAdsConfigRepository).FullName!, new MyDefine("InplayAdsConfig", "{\"unlocked\":true,\"placements\":{\"game_play\":[\"GadsmeCanvasLeaderboard728x90\",\"GadsmeCanvasMobileLeaderboard320x50\",\"AdvertyInMenuUnitLandscape\",\"AdvertyInMenuUnitWideLandscape\"],\"game_win\":[\"GadsmeCanvasVideo4x3\",\"GadsmeCanvasVideo16x9\",\"GadsmeCanvasMediumRectangle300x250\",\"GadsmeCanvasBillboard970x250\",\"GadsmeCanvasLeaderboard728x90\",\"GadsmeCanvasMobileLeaderboard320x50\",\"GadsmeCanvasBannerSquare320x320\",\"AdvertyInMenuUnitBox\",\"AdvertyInMenuUnitLandscape\",\"AdvertyInMenuUnitWideLandscape\"]}}") },
            { typeof(InAppUpdateConfigRepository).FullName!, new MyDefine("InAppUpdateConfig", "{\"unlocked\":true,\"minVersionCode\": 240513156}") },
#if CHEAT_TIME_DETECTOR
            { typeof(CheatTimeConfigRepository).FullName!, new MyDefine("CheatTimeConfig", "{\"unlocked\":true,\"url\":\"https://www.google.com\",\"allowedOffset\":30}")},
#endif
            { typeof(MultipleBannerAdsConfigRepository).FullName!, new MyDefine("MultipleBannerAdsConfig", "{\"unlocked\":false}") },
            { typeof(MultipleInterstitialAdsConfigRepository).FullName!, new MyDefine("MultipleInterstitialAdsConfig", "{\"unlocked\":false}") },
            { typeof(MultipleRewardedAdsConfigRepository).FullName!, new MyDefine("MultipleRewardedAdsConfig", "{\"unlocked\":false}") },
            
            // Read/Write Repository.
            { typeof(VibrationEnableRepository).FullName!, new MyDefine("Vibration", "true") },
            { typeof(TrackingAuthorizationRepository).FullName!, new MyDefine("TrackingAuthorizationRepository", "-1") },
            { typeof(StoreRatingShownCountRepository).FullName!, new MyDefine("StoreRatingShownCountRepository", "0") },
            { typeof(SessionCountRepository).FullName!, new MyDefine("SessionCountRepository", "0") },
            { typeof(SessionRepository).FullName!, new MyDefine("SessionRepository", "{}") },
            { typeof(PremiumEnableRepository).FullName!, new MyDefine("PremiumEnableRepository", "false") },
            { typeof(ParentControlRepository).FullName!, new MyDefine("ParentControlRepository", "{\"yearOfBirth\":2020,\"gender\":-1}") },
            { typeof(ParentControlStepRepository).FullName!, new MyDefine("ParentControlStepRepository", "{\"step\": -1}") },
            { typeof(LevelRepository).FullName!, new MyDefine("LevelRepository", "1") },
            { typeof(TrackingLevelRepository).FullName!, new MyDefine("TrackingLevelRepository", "-1") },
            { typeof(InAppUpdateRepository).FullName!, new MyDefine("InAppUpdateRepository", "{}") },
            { typeof(GameInterruptRepository).FullName!, new MyDefine("GameInterruptRepository", "{}") },
            { typeof(FirebaseRepository).FullName!, new MyDefine("FirebaseRepository", "{}") },
            { typeof(AudioEnableRepository).FullName!, new MyDefine("AudioEnableRepository", "true") },
            { typeof(TemporaryAudioEnableRepository).FullName!, new MyDefine("TemporaryAudioEnableRepository", "true") },
            { typeof(AppMetricaRepository).FullName!, new MyDefine("AppMetricaRepository", "{}") },
            { typeof(InterstitialAdRepository).FullName!, new MyDefine("InterstitialAdRepository", "{}") },
            { typeof(MrecPositionRepository).FullName!, new MyDefine("MrecPositionRepository", "{}") },
#if ADJUST
            { typeof(AdjustRepository).FullName!, new MyDefine("AdjustRepository", "{}") }
#endif
        };
        
        static RepositoryScanner()
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
            try
            {   
                var repositoryTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.GetCustomAttributes(typeof(RepositoryAttribute), false).Length > 0)
                    .ToArray();

                var userDefaultPath = Path.Combine(Application.dataPath, "Resources/Repository/RepositoryConfigurations.json");
                var configDefaultPath = Path.Combine(Application.dataPath, "Resources/Repository/RemoteConfigConfigurations.json");
                var oldUserDefault = _ReadDefaultValueFile(userDefaultPath);
                var oldConfigDefault = _ReadDefaultValueFile(configDefaultPath);

                var configDefault = new Dictionary<string, Dictionary<string, string>>();
                var userDefault = new Dictionary<string, Dictionary<string, string>>();
                foreach (var t in repositoryTypes)
                {
                    if (typeof(MonoBehaviour).IsAssignableFrom(t))
                        Debug.LogError($"{Tag}->typeof(MonoBehaviour).IsAssignableFrom({t.FullName})");

                    if (IgnoredRepositories.Contains(t.Name)) continue;

                    var classFullName = t.FullName!;
                    if (classFullName.EndsWith("ConfigRepository"))
                    {
                        DefaultRepositoryValues.TryGetValue(classFullName, out var definedDefaultValue);
                        var alreadyHave = oldConfigDefault.TryGetValue(classFullName, out var entry);
                        var name = alreadyHave ? entry["name"] : definedDefaultValue?.Name ?? t.Name;
                        var defaultValue = alreadyHave ? entry["default_value"] : definedDefaultValue?.DefaultValue ?? "";
                        configDefault.Add(classFullName, new Dictionary<string, string>()
                        {
                            { "name", name },
                            { "default_value", defaultValue }
                        });
                        oldConfigDefault.Remove(classFullName);
                    }
                    else
                    {
                        DefaultRepositoryValues.TryGetValue(classFullName, out var definedDefaultValue);
                        var alreadyHave = oldUserDefault.TryGetValue(classFullName, out var entry);
                        var name = alreadyHave ? entry["name"] : definedDefaultValue?.Name ?? t.Name;
                        var defaultValue = alreadyHave ? entry["default_value"] : definedDefaultValue?.DefaultValue ?? "";
                        userDefault.Add(classFullName, new Dictionary<string, string>()
                        {
                            { "name", name },
                            { "default_value", defaultValue }
                        });
                        oldUserDefault.Remove(classFullName);
                    }
                }
                
                foreach (var kv in oldConfigDefault) configDefault.Add(kv.Key, kv.Value);
                foreach (var kv in oldUserDefault) userDefault.Add(kv.Key, kv.Value);
                
                var isEditorLoggerEnabled = SharedEditorUtils.IsEditorLoggerEnabled();

                _ = SharedEditorUtils.WriteProjectRelativeFile("Assets/Resources/Repository/RepositoryConfigurations.json", JsonConvert.SerializeObject(userDefault, Formatting.Indented), !isEditorLoggerEnabled);
                _ = SharedEditorUtils.WriteProjectRelativeFile("Assets/Resources/Repository/RemoteConfigConfigurations.json", JsonConvert.SerializeObject(configDefault, Formatting.Indented), !isEditorLoggerEnabled);
            }
            finally
            {
                EditorApplication.delayCall -= OnAfterDomainReload;
            }
        }

        private static Dictionary<string, Dictionary<string, string>> _ReadDefaultValueFile(string filePath)
        {
            if (!File.Exists(filePath)) return new  Dictionary<string, Dictionary<string, string>>();
            var str = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(str);
        }
    }
}