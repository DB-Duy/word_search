using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Repository.Default
{
    public class DefaultRepository : IDefaultRepository, ISharedUtility
    {
        private const string Tag = "DefaultRepository";
        private const string Repo = "Repository/RepositoryConfigurations";
        private const string Config = "Repository/RemoteConfigConfigurations";

        private readonly Dictionary<string, Dictionary<string, string>> _dict;
        
        public DefaultRepository(bool isRemoteConfig)
        {
            var filePath = isRemoteConfig ? Config : Repo;
            var textAsset = Resources.Load<TextAsset>(filePath);
            _dict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(textAsset.text);
        }
        
        // {"name", t.FullName},
        // {"default_value", defaultValue}

        public string GetName(Type type)
        {
            var key = type.FullName;
            if (string.IsNullOrEmpty(key)) throw new Exception($"{Tag}->GetName. string.IsNullOrEmpty(key)");
            return _dict[key]["name"];
        }

        public bool GetBool(Type type)
        {
            var str = GetString(type);
            return !string.IsNullOrEmpty(str) && str is "true" or "1";
        }

        public string GetString(Type type)
        {
            var key = type.FullName;
            if (string.IsNullOrEmpty(key)) throw new Exception($"{Tag}->GetString. string.IsNullOrEmpty(key)");
            var v = _dict[key]["default_value"];
            return v is "null" or "" ? null : v;
        }

        public int GetInt(Type type)
        {
            var str = GetString(type);
            if (string.IsNullOrEmpty(str)) return 0;
            str = str.Trim();
            try
            {
                return int.Parse(str);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return 0;
            }
        }

        public long GetLong(Type type)
        {
            var str = GetString(type);
            if (string.IsNullOrEmpty(str)) return 0;
            str = str.Trim();
            try
            {
                return long.Parse(str);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return 0;
            }
        }

        public T GetObject<T>(Type type)
        {
            var str = GetString(type);
            if (string.IsNullOrEmpty(str)) return default;
            str = str.Trim();
            try
            {
                this.LogInfo(nameof(type), type?.Name, nameof(str), str);
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }
    }
}