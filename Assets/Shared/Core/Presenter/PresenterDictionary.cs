using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Core.Presenter
{
    public class PresenterDictionary<T> : IPresenter<T> where T : MonoBehaviour
    {
        private readonly Dictionary<string, IPresenter<T>> _presenters;

        public PresenterDictionary(Dictionary<string, IPresenter<T>> presenters)
        {
            _presenters = presenters;
        }

        public IEnumerator Present(T o)
        {
            var key = o.GetType().FullName;
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"string.IsNullOrEmpty(key) for {o}");
            if (!_presenters.ContainsKey(key)) yield break;
            _presenters[key].Present(o);
        }
    }
    
    public class PresenterDictionary<F, T> : IPresenter<F, T> 
        where F : MonoBehaviour
        where T : MonoBehaviour
    {
        private readonly Dictionary<string, IPresenter<F, T>> _presenters;

        public PresenterDictionary(Dictionary<string, IPresenter<F, T>> presenters)
        {
            _presenters = presenters;
        }

        public IEnumerator Present(F f, T t)
        {
            var key = $"{f.GetType().FullName}->{t.GetType().FullName}";
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"string.IsNullOrEmpty(key) for {f}, {t}");
            if (!_presenters.ContainsKey(key)) yield break;
            _presenters[key].Present(f, t);
        }
    }
}