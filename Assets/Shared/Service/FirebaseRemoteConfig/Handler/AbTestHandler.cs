using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Game;

namespace Shared.Service.FirebaseRemoteConfig.Handler
{
    [Service]
    public class AbTestHandler : IHandler<Dictionary<string, string>>, ISharedUtility
    {
        private const string Key = "ab_variant";
        
        public void Handle(Dictionary<string, string> remoteConfigData)
        {
            var filteredDict = remoteConfigData.Where(c => c.Value.Contains(Key)).ToDictionary(c => c.Key, c => c.Value);
            if (filteredDict.Count <= 0) return;
            foreach (var c in filteredDict)
            {
                var jo = JObject.Parse(c.Value);
                var value = jo.GetValue(Key)!.Value<string>();
                this.Track(new AbTestParams(c.Key, value));
            }
        }
    }
}