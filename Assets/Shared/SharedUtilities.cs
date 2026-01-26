using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Shared.Core.IoC;
using Shared.View.Ad;
using Shared.View.Fps;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// using System.Runtime.InteropServices;

namespace Shared
{
    public static class SharedUtilities
    {
        public static GameObject SharedGameObject { get; set; }
        public static GameObject GetSharedGameObject(this ISharedUtility u) => SharedGameObject;
#if UNITY_ANDROID
        private const string ClassUserInfoController = "com.unity3d.player.UserInfoController";
        private static readonly AndroidJavaClass UserInfoController = new(ClassUserInfoController);
        private static string _androidId;
        public static string GetAndroidId(this ISharedUtility u)
        {
            if (string.IsNullOrEmpty(_androidId)) 
                _androidId = UserInfoController.CallStatic<string>("getAndroidId");
            return _androidId;
        }

        public static AndroidJavaObject GetDefaultSharedPreferences(this ISharedUtility u)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var preferenceManagerClass = new AndroidJavaClass("android.preference.PreferenceManager");
            return preferenceManagerClass.CallStatic<AndroidJavaObject>("getDefaultSharedPreferences", currentActivity);
        }

        public static AndroidJavaObject GetCurrentActivity(this ISharedUtility u)
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
#endif
        
#if UNITY_WEBGL
        [DllImport("__Internal")] private static extern bool CheckNetworkStatus();
#endif
        public static bool IsInternetReachability()
        {
#if UNITY_ANDROID || UNITY_IOS
            return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
#elif UNITY_WEBGL
            return CheckNetworkStatus();
#else
            return true;
#endif
        }

        public static bool IsTablet(this ISharedUtility i) => IsTabletDevice;
        
        private static bool? _isTablet = null;
        public static bool IsTabletDevice => _isTablet ??= _IsTablet();
        
        private static bool _IsTablet()
        {
            float diagonalInches = 4;

            if (Screen.dpi > 0)
            {
                var screenWidth = Screen.width / Screen.dpi;
                var screenHeight = Screen.height / Screen.dpi;
                diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
            }

            var aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);

            return (diagonalInches > 6.5f && aspectRatio < 1.7f);
        }

        public static bool IsLowEndDevice(this ISharedUtility i)
        {
            return SystemInfo.systemMemorySize < 1024 * 3; // 3GB RAM = 1024 * 3
        }

        private static Dictionary<string, string> _versionDict;

        private static Dictionary<string, string> GetVersionDict()
        {
            if (_versionDict != null) return _versionDict;
            var version = Resources.Load<TextAsset>("version");
            _versionDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(version.text);
            return _versionDict;
        }

        public static string GetVersionName(this ISharedUtility u) => GetVersionDict()["v"];
        public static string GetVersionCode(this ISharedUtility u) => GetVersionDict()["c"];
        
        [Conditional("LOG_INFO")]
        public static void ShowFps(this ISharedUtility i)
        {
            var fpsGameObject = new GameObject();
            Object.DontDestroyOnLoad(fpsGameObject);
            fpsGameObject.AddComponent<FpsView>();
        }

        public static T GetOrAddComponent<T>(this GameObject o) where T : Behaviour
        {
            var t = o.GetComponent<T>();
            if (t == null) t = o.AddComponent<T>();
            return t;
        }

        public static void SetStretchVerticalAndHorizontal(this Transform nt)
        {
            var t = (RectTransform)nt;
            t.anchorMin = new Vector2(0, 0); // Bottom-left corner
            t.anchorMax = new Vector2(1, 1); // Top-right corner
            t.offsetMin = Vector2.zero;
            t.offsetMax = Vector2.zero;
        }
        
        public static void SetLocalPosition(this MonoBehaviour[] me, Vector3 localPosition)
        {
            foreach (var i in me)
                i.transform.localPosition = localPosition;
        }
        
        public static bool IsOnScreenOverlapOneOf(this RectTransform me, params RectTransform[] others)
        {
            return others.Any(me.IsOnScreenOverlap);
        }
        
        public static bool IsOnScreenOverlap(this RectTransform me, RectTransform other)
        {
            if (!other.gameObject.activeInHierarchy) return false;
            // Get world-space rectangles
            var myRect = me.ToWorldRect();
            var otherRect = other.ToWorldRect();

            // Check if the rectangles overlap
            return myRect.Overlaps(otherRect);
        }

        public static Rect ToWorldRect(this RectTransform me)
        {
            var corners = new Vector3[4];
            me.GetWorldCorners(corners);

            // Bottom left corner (min) and top right corner (max)
            Vector2 min = corners[0];
            Vector2 max = corners[2];

            return new Rect(min, max - min);
        }

        public static void SetActive(this MonoBehaviour[] me, bool active)
        {
            foreach (var i in me)
                i.gameObject.SetActive(active);
        }
        
        public static void SetActive(this GameObject[] me, bool active)
        {
            foreach (var i in me)
                i.SetActive(active);
        }
        
        public static void SetActive(this List<MonoBehaviour> me, bool active)
        {
            foreach (var i in me)
                i.gameObject.SetActive(active);
        }
        
        public static void SetActive(this List<GameObject> me, bool active)
        {
            foreach (var i in me)
                i.SetActive(active);
        }

        public static void AddRedImage(this RectTransform me)
        {
            var image = me.gameObject.GetComponent<Image>();
            if (image != null) return;
            image = me.gameObject.AddComponent<Image>();
            image.color = Color.red;
        }
        
        public static void RemoveRedImage(this RectTransform me)
        {
            var image = me.gameObject.GetComponent<Image>();
            if (image == null) return;
            Object.Destroy(image);
        }

        public static void FullFillHeightByScale(this RectTransform me, RectTransform other)
        {
            var scaleH = other.rect.height / me.rect.height;
            
            var width = me.rect.width * scaleH;
            if (width > other.rect.width)
            {
                var scaleW = other.rect.width / me.rect.width;
                me.localScale = new Vector3(scaleW, scaleW, scaleW);
                return;
            }
            me.localScale = new Vector3(scaleH, scaleH, scaleH);
        }
        
        public static void FullFillRectHeightByScale(this SpriteRenderer me, RectTransform rect)
        {
            if (me == null || rect == null) return;

            // Get the world size of the UI element
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            var width = Vector3.Distance(corners[0], corners[3]); // Left to Right
            var height = Vector3.Distance(corners[0], corners[1]); // Bottom to Top

            // Get the sprite's original size
            Vector2 spriteSize = me.sprite.bounds.size;

            var scaleH = height / spriteSize.y;
            // Scale the sprite to match the RectTransform
            me.transform.localScale = new Vector3(scaleH, scaleH, scaleH);
        }
        
        public static List<string> FilterByContains(this List<string> me, string filter)
        {
            return me.Where(i => i != null && i.Contains(filter)).ToList();
        }
        
        /// <summary>
        /// iPhone 7Plus: safeArea: (x:0.00, y:0.00, width:1080.00, height:1920.00)
        /// scaleFactor: 3, nativeScaleFactor: 2,608696
        /// </summary>
        public static Vector2 GetDpPosition(this RectTransform rectTransform, Camera uiCamera, float widthInDp, float heightInDp)
        {
#if UNITY_ANDROID
            var screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.position);
            var flippedY = Screen.height - screenPos.y;
            var screenAndroid = new Vector2(screenPos.x, flippedY);
            var w = ConvertDpToPx(widthInDp);
            var h = ConvertDpToPx(heightInDp);
            return new Vector2(screenAndroid.x - w / 2, screenAndroid.y - h / 2);
#else
            if (Screen.safeArea.y < 1)
            {
                // Device thường.
                var nativeScale = IosScaleFactorPlugin.GetNativeScaleFactor();
                var scale = IosScaleFactorPlugin.GetScaleFactor();
                var pixelPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.position);
                var flippedY = Screen.height - pixelPosition.y;
                pixelPosition = new Vector2(pixelPosition.x, flippedY);
            
                var pointPosition = pixelPosition / nativeScale;
                var logicalPointPosition = pointPosition * scale;
                var w = widthInDp * scale;
                var h = heightInDp * scale;
                return new Vector2(logicalPointPosition.x - w / 2, logicalPointPosition.y - h / 2);    
            }
            else
            {
                // Notch Device.
                var nativeScale = IosScaleFactorPlugin.GetNativeScaleFactor();
                var scale = IosScaleFactorPlugin.GetScaleFactor();
                var topInset = IosScaleFactorPlugin.GetSafeAreaTopInset() * scale;
                var pixelPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.position);
                var flippedY = Screen.height - pixelPosition.y;
                pixelPosition = new Vector2(pixelPosition.x, flippedY);

                var pointPosition = pixelPosition / nativeScale;
                var logicalPointPosition = pointPosition * scale;
                var w = widthInDp * scale;
                var h = heightInDp * scale;
                return new Vector2(logicalPointPosition.x - w / 2, logicalPointPosition.y - topInset * 2 - h / 2);
            }
#endif
        }
        
        public static float ConvertPxToDp(float px)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject resources = activity.Call<AndroidJavaObject>("getResources"))
            using (AndroidJavaObject metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics"))
            {
                float density = metrics.Get<float>("density");
                return px / density;
            }
#elif UNITY_IOS && !UNITY_EDITOR
            return IosScaleFactorPlugin.ConvertPxToDpUsingMaxFactor(px);
#else
            return px; // fallback
#endif
        }
        
        public static float ConvertDpToPx(float dp)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject resources = activity.Call<AndroidJavaObject>("getResources"))
            using (AndroidJavaObject metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics"))
            {
                int dpi = metrics.Get<int>("densityDpi");
                float density;
                var config = ProjectInstaller.Config;
                if (config.FixedDpi > 0 && dpi > config.FixedDpi)
                {
                    density = config.FixedDpi / 160f;
                }
                else
                {
                    density = metrics.Get<float>("density");
                }
                return dp * density;
            }
#elif UNITY_IOS && !UNITY_EDITOR
            return IosScaleFactorPlugin.ConvertDpToPxUsingMaxFactor(dp);
#else
            return dp; // fallback for Editor or non-Android platforms
#endif
        }
    }
}