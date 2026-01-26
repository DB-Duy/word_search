using SharedEditor.Editor.Utils;
using UnityEditor;

namespace Shared.Service.ParentControl.Editor
{
    [InitializeOnLoad]
    public class HyBidPluginGenerator
    {
        private const string IosHFilePath = "Assets/Plugins/iOS/HyBidPlugin.h";
        private const string IosHFileContent = @"
#import <Foundation/Foundation.h>
@interface HyBidPlugin : NSObject
+ (void)setAge:(NSInteger)age gender:(NSInteger)gender;
@end
";
        private const string IosMFilePath = "Assets/Plugins/iOS/HyBidPlugin.m";
        private const string IosMFileContent = @"
#import ""HyBidPlugin.h""
#import <HyBid/HyBid.h>

#if __has_include(<HyBid/HyBid-Swift.h>)
    #import <UIKit/UIKit.h>
    #import <HyBid/HyBid-Swift.h>
#else
    #import <UIKit/UIKit.h>
    #import ""HyBid-Swift.h""
#endif

@implementation HyBidPlugin
+ (void)setAge:(NSInteger)age gender:(NSInteger)gender {
    NSLog(@""[ParentControl,HyBid] HyBidPlugin->setAge: %d"", age);
    HyBidTargetingModel *targeting = [[HyBidTargetingModel alloc] init];
    targeting.age = [NSNumber numberWithInteger:age];
    if (gender == 0) {
        targeting.gender = @""m"";
        NSLog(@""[ParentControl,HyBid] HyBidPlugin->setGender: m"");
    }
    else if (gender == 1) {
        targeting.gender = @""f"";
        NSLog(@""[ParentControl,HyBid] HyBidPlugin->setGender: f"");
    } else {
        NSLog(@""[ParentControl,HyBid] HyBidPlugin->ignoreUndefinedGender: %d"", gender);
    }
    [HyBid setTargeting:targeting];
}
@end

#ifdef __cplusplus
extern ""C"" {
#endif
    void _SetHyBidAgeAndGender(int age, int gender) {
        [HyBidPlugin setAge:age gender:gender];
    }
#ifdef __cplusplus
}
#endif
";
        private const string AndroidFilePath = "Assets/Plugins/Android/HyBidPlugin.java";
        private const string AndroidFileContent = @"
package com.unity3d.player;

import net.pubnative.lite.sdk.HyBid;
import android.util.Log;

import java.util.HashMap;
import java.util.Map;

public class HyBidPlugin {

    private static final String TAG = ""HyBidPlugin"";
    public static Map<Integer, String> genderMap = new HashMap<>() {{
        put(0, ""male"");
        put(1, ""female"");
    }};

    public static void setAgeAndGender(int age, int gender) {
        Log.i(TAG, ""[ParentControl,HyBid] HyBidPlugin->setAge "" + age);
        HyBid.setAge(String.valueOf(age));
        if (genderMap.containsKey(gender)) {
            var sdkgender = genderMap.get(gender);
            HyBid.setGender(sdkgender);
        } else {
            Log.i(TAG, ""[ParentControl,HyBid] HyBidPlugin->ignoreUndefinedGender "" + gender);
        }
    }
}
";
        static HyBidPluginGenerator()
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
#if HY_BID && UNITY_IOS
            SharedEditorUtils.WriteContentToFile(IosHFilePath, IosHFileContent);
            SharedEditorUtils.WriteContentToFile(IosMFilePath, IosMFileContent);
#elif !HY_BID && UNITY_IOS
            SharedEditorUtils.DeleteFile(IosHFilePath);
            SharedEditorUtils.DeleteFile(IosMFilePath);
#elif HY_BID && UNITY_ANDROID
            SharedEditorUtils.WriteContentToFile(AndroidFilePath, AndroidFileContent);
#elif !HY_BID && UNITY_ANDROID
            SharedEditorUtils.DeleteFile(AndroidFilePath);
#endif
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
    }
}