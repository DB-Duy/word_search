using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Repository.ResourceJsonType
{
    /// <summary>
    /// Used for json object that like config.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TwoLevelJsonResourceRepository<T> : IJsonResourceRepository<T>
    {
        private const string Tag = "TwoLevelJsonResourceRepository";

        private readonly string _filePath;
        private readonly string _keyName;
        
        private T _t;
        public bool IsLoaded => _t != null;
        
        protected TwoLevelJsonResourceRepository(string keyName)
        {
            _filePath = TypeUtils.ResoleEntityResourceFullname<T>();
            _keyName = keyName;
        }
        
        public TwoLevelJsonResourceRepository(string filePath, string keyName)
        {
            _filePath = filePath;
            _keyName = keyName;
        }

        public void Load()
        {
            if (_t != null)
            {
                SharedLogger.LogError($"[{SharedLogTag.Repository}] {Tag}->Load: _t != null");
                return;
            }

            var textAsset = Resources.Load<TextAsset>(_filePath);
            if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
            {
                SharedLogger.LogError($"[{SharedLogTag.Repository}] {Tag}->Constructor: textAsset == null || string.IsNullOrEmpty(textAsset.text)");
                return;
            }
            SharedLogger.LogJson(SharedLogTag.Repository, $"{Tag}->Constructor: {textAsset.text}");
            var twoLevelDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(textAsset.text);
            if (!twoLevelDict.ContainsKey(_keyName))
                SharedLogger.LogError($"[{SharedLogTag.Repository}] {Tag}->Load: !twoLevelDict.ContainsKey({_keyName})");
            var resultDict = twoLevelDict[_keyName];
            var resultJson = JsonConvert.SerializeObject(resultDict);
            _t = JsonConvert.DeserializeObject<T>(resultJson);
        }

        public T Get()
        {
            if (!IsLoaded) Load();
            return _t;
        }
    }
}