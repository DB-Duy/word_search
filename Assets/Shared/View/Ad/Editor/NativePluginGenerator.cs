using System.IO;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Shared.View.Ad.Editor
{
    [InitializeOnLoad]
    public class NativePluginGenerator
    {
        private const string content = @"
#import <UIKit/UIKit.h>

extern ""C"" {
    float _GetScreenScaleFactor() {
        return (float)[[UIScreen mainScreen] scale];
    }

    float _GetScreenNativeScaleFactor() {
        return (float)[[UIScreen mainScreen] nativeScale];
    }

    float _GetScreenWidthInPoints() {
        return (float)[[UIScreen mainScreen] bounds].size.width;
    }

    float _GetScreenHeightInPoints() {
        return (float)[[UIScreen mainScreen] bounds].size.height;
    }

    float _GetScreenWidthInPixels() {
        return (float)[[UIScreen mainScreen] nativeBounds].size.width;
    }

    float _GetScreenHeightInPixels() {
        return (float)[[UIScreen mainScreen] nativeBounds].size.height;
    }

    float _GetSafeAreaTopInset() {
        UIWindow* window = UIApplication.sharedApplication.keyWindow;
        if (@available(iOS 11.0, *)) {
            return (float)window.safeAreaInsets.top;
        } else {
            return 0.0f;
        }
    }

    float _GetStatusBarHeightInPoints() {
        if (@available(iOS 13.0, *)) {
            UIWindow *window = UIApplication.sharedApplication.windows.firstObject;
            return window.windowScene.statusBarManager.statusBarFrame.size.height;
        } else {
            return UIApplication.sharedApplication.statusBarFrame.size.height;
        }
    }

    // Trả về khoảng cách từ gốc Unity (trên cùng màn hình) đến gốc của view mà ad được render
    float _GetNativeAdOffsetToTopInUnityPixels() {
         UIWindow *window = nil;

        // iOS 13+ dùng connectedScenes
        if (@available(iOS 13.0, *)) {
            for (UIScene *scene in UIApplication.sharedApplication.connectedScenes) {
                if (scene.activationState == UISceneActivationStateForegroundActive &&
                    [scene isKindOfClass:[UIWindowScene class]]) {
                    for (UIWindow *w in ((UIWindowScene *)scene).windows) {
                        if (w.isKeyWindow) {
                            window = w;
                            break;
                        }
                    }
                }
            }
        }

        // Fallback cho iOS < 13
        if (!window) {
            window = UIApplication.sharedApplication.keyWindow;
        }

        if (!window) {
            NSLog(@""❌ UIWindow not found"");
            return 0.0f;
        }

        UIView *view = window.rootViewController.view;
        if (!view) {
            NSLog(@""❌ rootViewController.view not found"");
            return 0.0f;
        }

        // Convert gốc của view sang hệ toạ độ của UIWindow
        CGRect frameInWindow = [view convertRect:view.bounds toView:window];
        CGFloat originYInPoints = frameInWindow.origin.y;

        CGFloat scale = UIScreen.mainScreen.nativeScale;
        float offsetInPixels = originYInPoints * scale;

        NSLog(@""✅ Offset from (0,0) to view = %.2f pts (%.2f px)"", originYInPoints, offsetInPixels);

        return offsetInPixels;
    }
}";     
        static NativePluginGenerator()
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
            
            _ = SharedEditorUtils.WriteProjectRelativeFile("Assets/Plugins/iOS/ScaleFactorPlugin.mm", content, silent: true);
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ NativePluginGenerator Domain reload complete!");
#endif
        }
    }
}