using SharedEditor.Editor.Utils;
using UnityEditor;

namespace Shared.Service.ParentControl.Editor
{
    [InitializeOnLoad]
    public class FyberInneractiveAdPluginGenerator
    {
        private const string PluginFolder = "Assets/Plugins/iOS";
        private const string IosHFilePath = "Assets/Plugins/iOS/FyberInneractiveAdPlugin.h";
        private const string IosHFileContent = @"
#import <Foundation/Foundation.h>
#import <IASDKCore/IAUserData.h>
@interface FyberInneractiveAdPlugin : NSObject
+ (void)setAge:(NSInteger)age gender:(NSInteger)gender;
@end
";
        private const string IosMFilePath = "Assets/Plugins/iOS/FyberInneractiveAdPlugin.m";
        private const string IosMFileContent = @"
#import ""FyberInneractiveAdPlugin.h""
@implementation FyberInneractiveAdPlugin
+ (void)setAge:(NSInteger)age gender:(NSInteger)gender {
    NSLog(@""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setAge: %ld"", age);
    IAUserGenderType genderType;
    switch (gender) {
        case 0:
            genderType = IAUserGenderTypeMale;
            NSLog(@""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setGender: IAUserGenderTypeMale"");
            break;
        case 1:
            genderType = IAUserGenderTypeFemale;
            NSLog(@""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setGender: IAUserGenderTypeFemale"");
            break;
        case 2:
            genderType = IAUserGenderTypeOther;
            NSLog(@""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setGender: IAUserGenderTypeOther"");
            break;
        case 3:
            genderType = IAUserGenderTypeUnknown;
            NSLog(@""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setGender: IAUserGenderTypeUnknown"");
            break;
        default:
            genderType = IAUserGenderTypeUnknown;
            NSLog(@""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setGender: IAUserGenderTypeUnknown"");
            break;
    }
    IAUserData *userData = [IAUserData build:^(id<IAUserDataBuilder> _Nonnull builder) {
        builder.age = age;
        builder.gender = genderType;
    }];
}
@end

#ifdef __cplusplus
extern ""C"" {
#endif
    void _SetFyberAgeAndGender(int age, int gender) {
        [FyberInneractiveAdPlugin setAge:age gender:gender];
    }
#ifdef __cplusplus
}
#endif
";
        
        private const string AndroidFilePath = "Assets/Plugins/Android/FyberInneractiveAdPlugin.java";
        private const string AndroidFileContent = @"
package com.unity3d.player;

import com.fyber.inneractive.sdk.external.InneractiveAdManager;
import com.fyber.inneractive.sdk.external.InneractiveUserConfig;
import android.util.Log;

import java.util.HashMap;
import java.util.Map;

public class FyberInneractiveAdPlugin {

    private static final String TAG = ""FyberInneractiveAdPlugin"";
    public static Map<Integer, InneractiveUserConfig.Gender> genderMap = new HashMap<>() {{
        put(0, InneractiveUserConfig.Gender.MALE);
        put(1, InneractiveUserConfig.Gender.FEMALE);
    }};

    public static void setAgeAndGender(int age, int gender)
    {
        Log.i(TAG, ""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setAgeAndGender: age="" + age + "", gender="" + gender);
        InneractiveUserConfig config = new InneractiveUserConfig();
        config.setAge(age);
        if (genderMap.containsKey(gender)) {
            var sdkgender = genderMap.get(gender);
            config.setGender(sdkgender);
            Log.i(TAG, ""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->setGender"" + sdkgender);
        } else {
            Log.i(TAG, ""[ParentControl,InneractiveAd,Fyber] FyberInneractiveAdPlugin->ignoreUndefinedGender "" + gender);
        }
        InneractiveAdManager.setUserParams(config);
    }
}
";

        static FyberInneractiveAdPluginGenerator()
        {
            EditorApplication.delayCall += OnAfterDomainReload;
        }
        
        private static void OnAfterDomainReload()
        {
            // Đảm bảo không thực thi khi đang Play hoặc sắp Play
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }      
#if FYBER && UNITY_IOS
            SharedEditorUtils.WriteContentToFile(IosHFilePath, IosHFileContent);
            SharedEditorUtils.WriteContentToFile(IosMFilePath, IosMFileContent);
#elif !FYBER && UNITY_IOS
            SharedEditorUtils.DeleteFile(IosHFilePath);
            SharedEditorUtils.DeleteFile(IosMFilePath);
#elif FYBER && UNITY_ANDROID
            SharedEditorUtils.WriteContentToFile(AndroidFilePath, AndroidFileContent);
#elif !FYBER && UNITY_ANDROID
            SharedEditorUtils.DeleteFile(AndroidFilePath);
#endif
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
    }
}