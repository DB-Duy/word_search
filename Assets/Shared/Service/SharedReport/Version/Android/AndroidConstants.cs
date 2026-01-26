namespace Shared.SharedReport.Version.Android
{
    public static class AndroidConstants
    {
        public class StaticJavaFunction
        {
            public enum Type
            {
                UnityPlugin,
                Adapter,
                Native
            }
            public string Name { get; }
            public string ClassName { get; }
            public string FunctionName { get; }
            public bool IsFunction { get; }

            public StaticJavaFunction(string className, string functionName, bool isFunction = true)
            {
                ClassName = className;
                FunctionName = functionName;
                IsFunction = isFunction;
            }
        }

        public static StaticJavaFunction AppLovin = new ("com.applovin.sdk.AppLovinSdk", "getVersion");
        public static StaticJavaFunction AppLovinLevelPlayAdapter = new ("com.ironsource.adapters.applovin.BuildConfig", "VERSION_NAME", isFunction: false);
        
        public static StaticJavaFunction Aps = new ("com.amazon.aps.ads.Aps", "getSdkVersion");
        public static StaticJavaFunction ApsLevelPlayAdapter = new ("com.ironsource.adapters.aps.BuildConfig", "VERSION_NAME", isFunction: false);
        
        public static StaticJavaFunction BidMachine = new ("io.bidmachine.BuildConfig", "VERSION_NAME", isFunction: false);
        public static StaticJavaFunction BidMachineLevelPlayAdapter = new ("com.ironsource.adapters.bidmachine.BuildConfig", "VERSION_NAME", isFunction: false);
        
        public static StaticJavaFunction Bigo = new ("sg.bigo.ads.BigoAdSdk", "getSDKVersionName");
        public static StaticJavaFunction BigoLevelPlayAdapter = new ("com.ironsource.adapters.bigo.BuildConfig", "VERSION_NAME", isFunction: false);
        
        public static StaticJavaFunction Chartboost = new ("com.chartboost.sdk.Chartboost", "getSDKVersion");
        public static StaticJavaFunction ChartboostLevelPlayAdapter = new ("com.ironsource.adapters.chartboost.BuildConfig", "VERSION_NAME", isFunction: false);
        
        public static StaticJavaFunction DigitalTurbine = new ("com.fyber.inneractive.sdk.external.InneractiveAdManager", "getVersion");
        public static StaticJavaFunction DigitalTurbineLevelPlayAdapter = new ("com.ironsource.adapters.fyber.BuildConfig", "VERSION_NAME", isFunction: false);
        
        public static StaticJavaFunction Admob = new ("com.ironsource.adapters.admob.AdMobAdapter", "getAdapterSDKVersion");
        public static StaticJavaFunction AdmobLevelPlayAdapter = new ("com.ironsource.adapters.admob.BuildConfig", "VERSION_NAME", isFunction: false);
        
    }
}