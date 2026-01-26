using SharedEditor.Editor.Utils;
using UnityEditor;

namespace Shared.Service.ParentControl.Editor
{
    [InitializeOnLoad]
    public class BidMachinePluginGenerator
    {
        private const string IosHFilePath = "Assets/Plugins/iOS/BidMachinePlugin.h";
        private const string IosHFileContent = @"
#import <Foundation/Foundation.h>
@interface BidMachinePlugin : NSObject
+ (void)setYearOfBirth:(NSInteger)yearOfBirth gender:(NSInteger)gender;
@end
";
        private const string IosMFilePath = "Assets/Plugins/iOS/BidMachinePlugin.m";
        private const string IosMFileContent = @"
#import ""BidMachinePlugin.h""
@import BidMachine;

@implementation BidMachinePlugin
+ (void)setYearOfBirth:(NSInteger)yearOfBirth gender:(NSInteger)gender {
    [BidMachineSdk.shared.targetingInfo populate:^(id<BidMachineTargetingInfoBuilderProtocol> builder) {
        [builder withUserYOB:(uint32_t)yearOfBirth];
        NSLog(@""[ParentControl,BidMachine] BidMachinePlugin->setYearOfBirth: %ld"", yearOfBirth);
        // Female, Male, Unknown
        if (gender == 0)
        {
            [builder withUserGender:BidMachineUserGenderMale];
            NSLog(@""[ParentControl,BidMachine] BidMachinePlugin->setGender: BidMachineUserGenderMale"");
        }
        else if (gender == 1)
        {
            [builder withUserGender:BidMachineUserGenderFemale];
            NSLog(@""[ParentControl,BidMachine] BidMachinePlugin->setGender: BidMachineUserGenderFemale"");
        }
        else
        {
            [builder withUserGender:BidMachineUserGenderUnknown];
            NSLog(@""[ParentControl,BidMachine] BidMachinePlugin->setGender: BidMachineUserGenderUnknown"");
        }
        }];
}
@end

#ifdef __cplusplus
extern ""C"" {
#endif
    void _SetBidMachineYearOfBirthAndGender(int yearOfBirth, int gender) {
        [BidMachinePlugin setYearOfBirth:yearOfBirth gender:gender];
    }
#ifdef __cplusplus
}
#endif
";
        private const string AndroidFilePath = "Assets/Plugins/Android/BidMachinePlugin.java";
        private const string AndroidFileContent = @"
package com.unity3d.player;

import java.util.HashMap;
import java.util.Map;
import android.util.Log;

import io.bidmachine.BidMachine;
import io.bidmachine.TargetingParams;
import io.bidmachine.utils.Gender;

public class BidMachinePlugin {
    private static final String TAG = ""BidMachinePlugin"";

    public static Map<Integer, Gender> genderMap = new HashMap<>() {{
        put(0, Gender.Male);
        put(1, Gender.Female);
        put(2, Gender.Omitted);
        put(3, Gender.Omitted);
    }};

    public static void setYearOfBirthAndGender(int yearOfBirth, int gender)
    {
        Log.i(TAG, ""[ParentControl,BidMachine] BidMachinePlugin->setYearOfBirthAndGender: yearOfBirth="" + yearOfBirth);
        TargetingParams targetingParams = new TargetingParams().setBirthdayYear(yearOfBirth);
        if (genderMap.containsKey(gender)) {
           var sdkgender = genderMap.get(gender);
           targetingParams.setGender(genderMap.get(sdkgender));
           Log.i(TAG, ""[ParentControl,BidMachine] BidMachinePlugin->setGender"" + sdkgender);
        } else {
            Log.i(TAG, ""[ParentControl,BidMachine] BidMachinePlugin->ignoreUndefinedGender "" + gender);
        }
        BidMachine.setTargetingParams(targetingParams);
    }
}
";
        static BidMachinePluginGenerator()
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
#if BID_MACHINE && UNITY_IOS
            SharedEditorUtils.WriteContentToFile(IosHFilePath, IosHFileContent);
            SharedEditorUtils.WriteContentToFile(IosMFilePath, IosMFileContent);
#elif !BID_MACHINE && UNITY_IOS
            SharedEditorUtils.DeleteFile(IosHFilePath);
            SharedEditorUtils.DeleteFile(IosMFilePath);
#elif BID_MACHINE && UNITY_ANDROID
            SharedEditorUtils.WriteContentToFile(AndroidFilePath, AndroidFileContent);
#elif !BID_MACHINE && UNITY_ANDROID
            SharedEditorUtils.DeleteFile(AndroidFilePath);
#endif
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
    }
}