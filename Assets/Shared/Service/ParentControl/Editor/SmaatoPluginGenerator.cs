using SharedEditor.Editor.Utils;
using UnityEditor;

namespace Shared.Service.ParentControl.Editor
{
    [InitializeOnLoad]
    public class SmaatoPluginGenerator
    {
        private const string IosHFilePath = "Assets/Plugins/iOS/SmaatoPlugin.h";
        private const string IosHFileContent = @"
#import <Foundation/Foundation.h>
@interface SmaatoPlugin : NSObject
+ (void)setAge:(NSInteger)age gender:(NSInteger)gender;
@end
";
        private const string IosMFilePath = "Assets/Plugins/iOS/SmaatoPlugin.m";
        private const string IosMFileContent = @"
#import ""SmaatoPlugin.h""
@import SmaatoSDKCore;
@implementation SmaatoPlugin

+ (void)setAge:(NSInteger)age gender:(NSInteger)gender {
    
    SmaatoSDK.userAge = @(age);
    NSLog(@""[ParentControl,Smaato] SmaatoPlugin->setAge %ld"", age);
    if (gender == 0) {
        SmaatoSDK.userGender = kSMAGenderMale;
        NSLog(@""[ParentControl,Smaato] SmaatoPlugin->setGender kSMAGenderMale"");
    }
    else if (gender == 1) {
        SmaatoSDK.userGender = kSMAGenderFemale;
        NSLog(@""[ParentControl,Smaato] SmaatoPlugin->setGender kSMAGenderFemale"");
    }
    else if (gender == 2) {
        SmaatoSDK.userGender = kSMAGenderOther;
        NSLog(@""[ParentControl,Smaato] SmaatoPlugin->setGender kSMAGenderOther"");
    } else {
        SmaatoSDK.userGender = kSMAGenderUnknown;
        NSLog(@""[ParentControl,Smaato] SmaatoPlugin->setGender kSMAGenderUnknown"");
    }
}

@end


#ifdef __cplusplus
extern ""C"" {
#endif
    void _SetSmaatoAgeAndGender(int age, int gender) {
        [SmaatoPlugin setAge:age gender:gender];
    }
#ifdef __cplusplus
}
#endif
";
        private const string AndroidFilePath = "Assets/Plugins/Android/SmaatoPlugin.java";
        private const string AndroidFileContent = @"
package com.unity3d.player;

import com.smaato.sdk.core.Gender;
import com.smaato.sdk.core.SmaatoSdk;
import android.util.Log;

import java.util.HashMap;
import java.util.Map;

public class SmaatoPlugin {
    private static final String TAG = ""SmaatoPlugin"";

    public static Map<Integer, Gender> genderMap = new HashMap<>() {{
        put(0, Gender.MALE);
        put(1, Gender.FEMALE);
        put(2, Gender.OTHER);
        put(3, Gender.OTHER);
    }};

    public static void setAgeAndGender(int age, int gender)
    {
        Log.i(TAG, ""[ParentControl,Smaato] SmaatoPlugin->setAge"" + age);
        SmaatoSdk.setAge(age);
        if (genderMap.containsKey(gender)) {
            var sdkgender = genderMap.get(gender);
            SmaatoSdk.setGender(sdkgender);
            Log.i(TAG, ""[ParentControl,Smaato] SmaatoPlugin->setGender"" + sdkgender);
        } else {
            Log.i(TAG, ""[ParentControl,Smaato] SmaatoPlugin->ignoreUndefinedGender "" + gender);
        }
    }
}
";
        static SmaatoPluginGenerator()
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
#if SMAATO && UNITY_IOS
            SharedEditorUtils.WriteContentToFile(IosHFilePath, IosHFileContent);
            SharedEditorUtils.WriteContentToFile(IosMFilePath, IosMFileContent);
#elif !SMAATO && UNITY_IOS
            SharedEditorUtils.DeleteFile(IosHFilePath);
            SharedEditorUtils.DeleteFile(IosMFilePath);
#elif SMAATO && UNITY_ANDROID
            SharedEditorUtils.WriteContentToFile(AndroidFilePath, AndroidFileContent);
#elif !SMAATO && UNITY_ANDROID
            SharedEditorUtils.DeleteFile(AndroidFilePath);
#endif
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
    }
}