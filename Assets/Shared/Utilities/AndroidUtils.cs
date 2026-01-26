using UnityEngine;

public static class AndroidUtils
{
    public static int GetVersionCode()
    {
#if PLATFORM_ANDROID
        if (Application.isEditor) return 0;
        AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager");
        string packageName = context.Call<string>("getPackageName");
        AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
        return packageInfo.Get<int>("versionCode");
#else
        return 0;
#endif
    }
    
    public static string GetSHA1Key()
    {
        if (Application.platform != RuntimePlatform.Android) return null;
        try
        {
            // Get the current activity
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Get the Java class you created in your Android plugin (SignatureUtil)
            using var signatureUtil = new AndroidJavaClass("com.unity3d.player.SignatureUtil");
            // Call the static Java method getSHA1Key
            var sha1Key = signatureUtil.CallStatic<string>("getSHA1Key", currentActivity);
            return sha1Key;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to get SHA1 key: " + e.Message);
        }

        return null;
    }
    
    public static string GetPackageName()
    {
        if (Application.platform != RuntimePlatform.Android) return null;
        try
        {
            // Get the current Android activity
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Get the package manager and package name from the current activity
            var packageName = currentActivity.Call<string>("getPackageName");

            return packageName;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to get package name: " + e.Message);
        }
        return null;
    }
}
