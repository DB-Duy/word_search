using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Core.IoC;
using Shared.Core.View.Scene;
using Shared.View.SharedInPlayAd;
using UnityEngine;

namespace Shared.Controller.SharedInPlayAd
{
    [DisallowMultipleComponent]
    public abstract class SharedInPlayAdController : IoCMonoBehavior, ISharedInPlayAdController, ISharedUtility
    {
        private readonly Dictionary<string, List<GameObject>> _adsDict = new();

        private void Start()
        {
            StartCoroutine(_Init());
        }

        protected abstract IEnumerator _Init();

        public T InitInPlayAdView<T>() where T : MonoBehaviour
        {
            var className = typeof(T).FullName;
            if (string.IsNullOrEmpty(className)) return null;
            if (!_adsDict.ContainsKey(className)) _adsDict.Add(className, new List<GameObject>());
            var createdOne = this.InstantiateSharedFeature<T>(this.transform);
            _adsDict[className].Add(createdOne.gameObject);
            createdOne.gameObject.name = className;
            return createdOne;
        }

        public T Request<T>() where T : MonoBehaviour
        {
            var className = typeof(T).FullName;
            if (string.IsNullOrEmpty(className)) return null;
            if (!_adsDict.ContainsKey(className)) return null;
            var r = _Get(className, requireReady: true);
            return r == null ? null : r.GetComponent<T>();
        }

        public GameObject RequestByClassName(params string[] classFullNames)
        {
            return classFullNames.Select(classFullName => _Get(classFullName, true)).FirstOrDefault(t => t != null);
        }

        public void Release<T>(T o) where T : MonoBehaviour
        {
            var className = typeof(T).FullName;
            if (string.IsNullOrEmpty(className)) return;
            _adsDict[className].Add(o.gameObject);
            var ot = o.transform;
            var mt = transform;
            ot.SetParent(mt);
            ot.position = mt.position;
        }

        // -------------------------------------------------------------------------
        // -------------------------------------------------------------------------
        private GameObject _Get(string className, bool requireReady)
        {
            if (!_adsDict.ContainsKey(className)) return null;
            if (_adsDict[className].Count <= 0) return null;
            return (from ad in _adsDict[className] let v = ad.GetComponent<SharedInPlayAdView>() where v.IsReady == requireReady select ad).FirstOrDefault();
        }
    }
}