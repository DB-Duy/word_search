using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Repository.Prefab
{
    public class FeaturePrefabRepository : IPrefabRepository, ISharedUtility
    {
        private const string Tag = "FeaturePrefabRepository";
        
        public virtual T GetOrLoad<T>() where T : MonoBehaviour
        {
            var path = TypeUtils.ResolvePrefabFilePath<T>();
            // "Shared.View.ParentControl.ParentControlDialog": "View.ParentControl.ParentControlDialog"
            if (path.StartsWith("Shared/"))
            {
                var overridePath = path.Replace("Shared/", string.Empty);
                var overridePrefab = Resources.Load<T>(overridePath);
                if (overridePrefab != null) return overridePrefab;
            }

            var prefab = Resources.Load<T>(path);
            if (prefab == null)
            {
                this.LogError(SharedLogTag.Repository, nameof(path), path);
                return default;
            }
            prefab.gameObject.SetActive(false);
            return prefab;
        }

        public virtual void Unload<T>() where T : MonoBehaviour
        {
            // Resources.UnloadAsset();
        }
    }
}