using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Entity.Config;
using Shared.Utils;
using UnityEngine;

namespace Shared.Repository.Config
{
    public class ConfigRepository : IConfigRepository
    {
        private const string Tag = "ConfigRepository";

#if GOOGLE_PLAY
        private const string Platform = "android";
#elif APPSTORE
        private const string Platform = "ios";
#elif APPSTORE_CHINA
        private const string Platform = "ios_china";
#elif HUAWEI
        private const string Platform = "huawei";
#elif FACEBOOK_INSTANT
        private const string Platform = "facebook";
#else
        private const string Platform = "unity_editor";
#endif
        private IConfig _config;

        private void _Initialize()
        {
            if (_config != null) return;
            var textAsset = Resources.Load<TextAsset>("Config/Config");
            var str = textAsset.text;
            if (string.IsNullOrEmpty(str))
                throw new System.Exception($"{Tag}->Initialize: ERROR: string.IsNullOrEmpty(str)");
            var aa = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(str);
            var bb = _FilterByPlatform(aa);
            var jsonStr = JsonConvert.SerializeObject(bb);
            SharedLogger.Log($"{Tag}->Initialize: {jsonStr}");
            _config = JsonConvert.DeserializeObject<Entity.Config.Config>(jsonStr);
        }

        private Dictionary<string, string> _FilterByPlatform(Dictionary<string, Dictionary<string, string>> aa)
        {
            var bb = new Dictionary<string, string>();
            foreach (var (key, value) in aa)
            {
                if (value.TryGetValue(Platform, out var realValue))
                {
                    bb.Add(key, realValue);
                }
            }

            return bb;
        }

        public IConfig Get()
        {
            _Initialize();
            return _config;
        }
    }
}