using SharedEditor.Editor.Utils;
using UnityEditor;

namespace Shared.Service.ParentControl.Editor
{
    [InitializeOnLoad]
    public class InmobiPluginGenerator
    {
        private const string IosHeaderPath = "Assets/Plugins/iOS/InMobiPlugin.h";
        private const string IosImplPath   = "Assets/Plugins/iOS/InMobiPlugin.m";

        private const string IosHeaderContent = @"
#import <Foundation/Foundation.h>
#import <InMobiSDK/InMobiSDK.h>

@interface InMobiPlugin : NSObject

+ (void)setAge:(NSInteger)age gender:(NSInteger)gender;
+ (void)updateGDPRConsent:(NSDictionary *)consent;
+ (void)setUSPrivacyString:(NSString *)usPrivacyString;

@end

#ifdef __cplusplus
extern ""C"" {
#endif
    void _SetInMobiAgeAndGender(int age, int gender);
    void _InMobiUpdateGDPRConsent(const char* jsonConsent);
    void _InMobiSetUSPrivacyString(const char* usPrivacyString);
#ifdef __cplusplus
}
#endif
";

        // Implementation
        private const string IosImplContent = @"
#import ""InMobiPlugin.h""

@implementation InMobiPlugin

+ (void)setAge:(NSInteger)age gender:(NSInteger)gender {
    [IMSdk setAge:age];
    NSLog(@""[ParentControl,InMobi] InMobiPlugin->setAge: %ld"", (long)age);

    if (gender == 0) {
        [IMSdk setGender:IMSDKGenderMale];
        NSLog(@""[ParentControl,InMobi] InMobiPlugin->setGender: IMSDKGenderMale"");
    } else if (gender == 1) {
        [IMSdk setGender:IMSDKGenderFemale];
        NSLog(@""[ParentControl,InMobi] InMobiPlugin->setGender: IMSDKGenderFemale"");
    } else {
        NSLog(@""[ParentControl,InMobi] InMobiPlugin->ignoreUndefinedGender: %ld"", (long)gender);
    }
}

+ (void)updateGDPRConsent:(NSDictionary *)consent {
    if (!consent) return;
    [IMSdk updateGDPRConsent:consent];
    NSLog(@""[Ump,InMobi] InMobiPlugin->updateGDPRConsent: %@"", consent);
}

+ (void)setUSPrivacyString:(NSString *)usPrivacyString {
    if (!usPrivacyString) return;
    [IMSdk setUSPrivacyString:usPrivacyString];
    NSLog(@""[Ump,InMobi] InMobiPlugin->setUSPrivacyString: %@"", usPrivacyString);
}

@end

#ifdef __cplusplus
extern ""C"" {
#endif

void _SetInMobiAgeAndGender(int age, int gender) {
    [InMobiPlugin setAge:age gender:gender];
}

void _InMobiUpdateGDPRConsent(const char* jsonConsent) {
    if (!jsonConsent) return;
    NSString *json = [NSString stringWithUTF8String:jsonConsent];
    if (!json) return;
    NSData *data = [json dataUsingEncoding:NSUTF8StringEncoding];
    if (!data) return;
    NSError *error = nil;
    id obj = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (!error && [obj isKindOfClass:[NSDictionary class]]) {
        [InMobiPlugin updateGDPRConsent:(NSDictionary *)obj];
    } else {
        NSLog(@""[Ump,InMobi] InMobiPlugin->_InMobiUpdateGDPRConsent parse error: %@"", error.localizedDescription);
    }
}

void _InMobiSetUSPrivacyString(const char* usPrivacyString) {
    if (!usPrivacyString) return;
    NSString *str = [NSString stringWithUTF8String:usPrivacyString];
    [InMobiPlugin setUSPrivacyString:str];
}

#ifdef __cplusplus
}
#endif
";

        private const string AndroidFilePath = "Assets/Plugins/Android/InMobiPlugin.java";
        private const string AndroidFileContent = @"
package com.unity3d.player;

import android.util.Log;
import com.inmobi.sdk.InMobiSdk;
import java.util.HashMap;
import java.util.Map;

public class InMobiPlugin {
    private static final String TAG = ""InMobiPlugin"";

    public static Map<Integer, InMobiSdk.Gender> genderMap = new HashMap<Integer, InMobiSdk.Gender>() {{
        put(0, InMobiSdk.Gender.MALE);
        put(1, InMobiSdk.Gender.FEMALE);
    }};

    public static void setAgeAndGender(int age, int gender) {
        Log.i(TAG, ""[ParentControl,InMobi] InMobiPlugin->setAge "" + age);
        InMobiSdk.setAge(age);

        if (genderMap.containsKey(gender)) {
            InMobiSdk.Gender sdkGender = genderMap.get(gender);
            InMobiSdk.setGender(sdkGender);
            Log.i(TAG, ""[ParentControl,InMobi] InMobiPlugin->setGender "" + sdkGender);
        } else {
            Log.i(TAG, ""[ParentControl,InMobi] InMobiPlugin->ignoreUndefinedGender "" + gender);
        }
    }
}
";

        static InmobiPluginGenerator() {
            EditorApplication.delayCall += OnAfterDomainReload;
        }

        private static void OnAfterDomainReload() {
            if (EditorApplication.isPlayingOrWillChangePlaymode) {
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }

#if INMOBI && UNITY_IOS
            SharedEditorUtils.WriteContentToFile(IosHeaderPath, IosHeaderContent);
            SharedEditorUtils.WriteContentToFile(IosImplPath, IosImplContent);
#elif !INMOBI && UNITY_IOS
            SharedEditorUtils.DeleteFile(IosHeaderPath);
            SharedEditorUtils.DeleteFile(IosImplPath);
#elif INMOBI && UNITY_ANDROID
            SharedEditorUtils.WriteContentToFile(AndroidFilePath, AndroidFileContent);
#elif !INMOBI && UNITY_ANDROID
            SharedEditorUtils.DeleteFile(AndroidFilePath);
#endif
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
    }
}