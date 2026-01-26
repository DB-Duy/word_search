using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Repository.ResourceJsonType
{  
    public class DictionaryResourceRepository<T> : IDictionaryResourceRepository<T>
    {
        private const string Tag = "DictionaryResourceRepository";

        private string _filePath;
        
        private Dictionary<string, T> _t;
        public bool IsLoaded => _t != null;
        
        protected DictionaryResourceRepository()
        {
            _filePath = TypeUtils.ResoleEntityResourceFullname<T>();
        }
        
        public DictionaryResourceRepository(string filePath)
        {
            _filePath = filePath;
        }

        public void Load()
        {
            if (_t != null)
            {
                SharedLogger.LogJson(SharedLogTag.Repository, $"{Tag}->Load: _t != null", nameof(_filePath), _filePath);
                return;
            }

            var textAsset = Resources.Load<TextAsset>(_filePath);
            if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
            {
                SharedLogger.LogError($"[{SharedLogTag.Repository}] {Tag}->Constructor: textAsset == null || string.IsNullOrEmpty(textAsset.text)");
                return;
            }
            SharedLogger.LogJson(SharedLogTag.Repository, $"{Tag}->Constructor", nameof(textAsset.text), textAsset.text);
            _t = JsonConvert.DeserializeObject<Dictionary<string, T>>(textAsset.text);
        }

        public T Get(string id, T defaultValue = default)
        {
            if (!IsLoaded) Load();
            return _t.Get(id, defaultValue);
        }
    }
}