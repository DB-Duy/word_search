using System.Collections.Generic;
using UnityEngine;

namespace Shared.View.Ad
{
    /// <summary>
    /// iPhone 7 Plus: iOS Device: {"scaleFactor":"3","nativeScaleFactor":"2,608696","ScreenWidthInPixels":"1080","ScreenHeightInPixels":"1920","ScreenWidthInPoints":"414","ScreenHeightInPoints":"736","Screen.width":"1080","Screen.height":"1920","Screen.safeArea":"(x:0.00, y:0.00, width:1080.00, height:1920.00)"}
    /// </summary>
    public static class IosScaleFactorPlugin
    {
#if UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetScreenScaleFactor();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetScreenNativeScaleFactor();
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetScreenWidthInPoints();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetScreenHeightInPoints();
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetScreenWidthInPixels();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetScreenHeightInPixels();
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetSafeAreaTopInset();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetNativeAdOffsetToTopInUnityPixels();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float _GetStatusBarHeightInPoints();
#else
        private static float _GetScreenScaleFactor() => 1.0f;
        private static float _GetScreenNativeScaleFactor() => 1.0f;
        
        private static float _GetScreenWidthInPoints() => Screen.width;
        private static float _GetScreenHeightInPoints() => Screen.height;
    
        private static float _GetScreenWidthInPixels() => Screen.width;
        private static float _GetScreenHeightInPixels() => Screen.height;
        
        private static float _GetSafeAreaTopInset() => 0f;
        private static float _GetNativeAdOffsetToTopInUnityPixels() => 0f;
        private static float _GetStatusBarHeightInPoints() => 0f;
#endif
        
        public static float GetScaleFactor()
        {
            return _GetScreenScaleFactor();
        }
        
        public static float GetNativeScaleFactor()
        {
            return _GetScreenNativeScaleFactor();
        }
        
        public static float ConvertDpToPx(float dp)
        {
            return dp * GetScaleFactor();
        }
        
        // Iphone7+: scaleFactor: 3, nativeScaleFactor: 2,608696
        public static float ConvertDpToPxUsingMaxFactor(float dp)
        {
            // var max = Mathf.Max(GetScaleFactor(), GetNativeScaleFactor());
            var max = GetNativeScaleFactor();
            return dp * max;
        }
        
        public static float ConvertPxToDpUsingMaxFactor(float px)
        {
            // var max = Mathf.Max(GetScaleFactor(), GetNativeScaleFactor());
            var max = GetNativeScaleFactor();
            return px / max;
        }
        
        public static float GetScreenWidthInPoints() => _GetScreenWidthInPoints();
        public static float GetScreenHeightInPoints() => _GetScreenHeightInPoints();
    
        public static float GetScreenWidthInPixels() => _GetScreenWidthInPixels();
        public static float GetScreenHeightInPixels() => _GetScreenHeightInPixels();
        
        public static float GetRatePointsOnPixels() => GetScreenWidthInPoints() / Screen.width;

        public static float GetSafeAreaTopInset() => _GetSafeAreaTopInset();
        public static float GetNativeAdOffsetToTopInUnityPixels() => _GetNativeAdOffsetToTopInUnityPixels();
        public static float GetStatusBarHeightInPoints() => _GetStatusBarHeightInPoints();
    }
}