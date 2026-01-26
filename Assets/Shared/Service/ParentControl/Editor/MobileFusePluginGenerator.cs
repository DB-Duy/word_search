using SharedEditor.Editor.Utils;
using UnityEditor;

namespace Shared.Service.ParentControl.Editor
{
    [InitializeOnLoad]
    public class MobileFusePluginGenerator
    {
        private const string IosHFilePath = "Assets/Plugins/iOS/MobileFusePlugin.h";
        private const string IosHFileContent = @"
#import <Foundation/Foundation.h>
@interface MobileFusePlugin : NSObject
+ (void)setAge:(NSInteger)age gender:(NSInteger)gender;
@end
";

        private const string IosMFilePath = "Assets/Plugins/iOS/MobileFusePlugin.m";
        private const string IosMFileContent = @"
#import ""MobileFusePlugin.h""
#import <MobileFuseSDK/MobileFuseSDK.h>

@implementation MobileFusePlugin
+ (void)setAge:(NSInteger)age gender:(NSInteger)gender {
    NSLog(@""[ParentControl,MobileFuse] MobileFusePlugin->setAge %ld"", age);
    [MobileFuseTargetingData setAge: (uint)age];
    if (gender == 0) {
        [MobileFuseTargetingData setGender: MOBILEFUSE_TARGETING_DATA_GENDER_MALE];
        NSLog(@""[ParentControl,MobileFuse] MobileFusePlugin->setGender MOBILEFUSE_TARGETING_DATA_GENDER_MALE"");
    } else if (gender == 1) {
        [MobileFuseTargetingData setGender: MOBILEFUSE_TARGETING_DATA_GENDER_FEMALE];
        NSLog(@""[ParentControl,MobileFuse] MobileFusePlugin->setGender MOBILEFUSE_TARGETING_DATA_GENDER_FEMALE"");
    } else if (gender == 2) {
        [MobileFuseTargetingData setGender: MOBILEFUSE_TARGETING_DATA_GENDER_OTHER];
        NSLog(@""[ParentControl,MobileFuse] MobileFusePlugin->setGender MOBILEFUSE_TARGETING_DATA_GENDER_OTHER"");
    } else {
        [MobileFuseTargetingData setGender: MOBILEFUSE_TARGETING_DATA_GENDER_UNKNOWN];
        NSLog(@""[ParentControl,MobileFuse] MobileFusePlugin->setGender MOBILEFUSE_TARGETING_DATA_GENDER_UNKNOWN"");
    }
}
@end

#ifdef __cplusplus
extern ""C"" {
#endif
    void _SetMobileFuseAgeAndGender(int age, int gender) {
        [MobileFusePlugin setAge:age gender:gender];
    }
#ifdef __cplusplus
}
#endif
";

        private const string AndroidFilePath = "Assets/Plugins/Android/MobileFusePlugin.java";
        private const string AndroidFileContent = @"
package com.unity3d.player;

import android.util.Log;
import com.mobilefuse.sdk.MobileFuseTargetingData;
import com.mobilefuse.sdk.user.Gender;
import java.util.HashMap;
import java.util.Map;

public class MobileFusePlugin {
    private static final String TAG = ""MobileFusePlugin"";

    public static Map<Integer, Gender> genderMap = new HashMap<>() {{
        put(0, Gender.MALE);
        put(1, Gender.FEMALE);
        put(2, Gender.OTHER);
        put(3, Gender.UNKNOWN);
    }};

    public static void setAgeAndGender(int age, int gender)
    {
        Log.i(TAG, ""[ParentControl,MobileFuse] MobileFusePlugin->setAgeAndGender: age="" + age);
        MobileFuseTargetingData.setAge(age);
        if (genderMap.containsKey(gender)) {
            var sdkGender = genderMap.get(gender);
            if (sdkGender == null) return;
            MobileFuseTargetingData.setGender(sdkGender);
            Log.i(TAG, ""[ParentControl,MobileFuse] MobileFusePlugin->setAgeAndGender: gender="" + sdkGender);
        } else {
            Log.i(TAG, ""[ParentControl,MobileFuse] MobileFusePlugin->ignoreUndefinedGender "" + gender);
        }
    }
}
";

        static MobileFusePluginGenerator()
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
            
#if MOBILE_FUSE && UNITY_IOS
            SharedEditorUtils.WriteContentToFile(IosHFilePath, IosHFileContent);
            SharedEditorUtils.WriteContentToFile(IosMFilePath, IosMFileContent);
#elif !MOBILE_FUSE && UNITY_IOS
            SharedEditorUtils.DeleteFile(IosHFilePath);
            SharedEditorUtils.DeleteFile(IosMFilePath);
#elif MOBILE_FUSE && UNITY_ANDROID
            SharedEditorUtils.WriteContentToFile(AndroidFilePath, AndroidFileContent);
#elif !MOBILE_FUSE && UNITY_ANDROID
            SharedEditorUtils.DeleteFile(AndroidFilePath);
#endif
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
    }
}