using Newtonsoft.Json;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Repository.ResourceJsonType
{
    public class JsonResourceRepository<T> : IJsonResourceRepository<T>
    {
        private const string Tag = "JsonResourceRepository";

        private readonly string _filePath;
        private readonly string _keyName;
        
        private T _t;
        public bool IsLoaded => _t != null;
        
        
        public JsonResourceRepository(string filePath)
        {
            _filePath = filePath;
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
            _t = JsonConvert.DeserializeObject<T>(textAsset.text);
        }

        public T Get()
        {
            if (!IsLoaded) Load();
            return _t;
        }
    }
}