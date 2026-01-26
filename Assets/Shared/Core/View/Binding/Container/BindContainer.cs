using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.Repository.BoolType;
using Shared.Core.Repository.IntType;
using Shared.Core.Repository.JsonType;
using Shared.Core.Repository.StringType;
using Shared.Core.View.Binding.Binder;
using Shared.Core.View.Binding.Formatter;
using Shared.Core.View.Binding.Presenter;
using Shared.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.Core.View.Binding.Container
{
    [Component]
    public class BindContainer : IBindContainer
    {
        private readonly Dictionary<MonoBehaviour, IBinder> _bindDict = new();
        private readonly Dictionary<GameObject, IBinder> _activeBinderDict = new();
        
        public bool Bind(TextMeshProUGUI t, IIntRepository r, ITextFormatter<int> formatter = null, IPresenter<IntTextBinder,int> presenter = null)
        {
            if (_bindDict.ContainsKey(t)) return false;
            _bindDict.Add(t, new IntTextBinder(t, r, formatter, presenter).Bind());
            return true;
        }

        public bool Bind(TextMeshProUGUI t, IStringRepository r, ITextFormatter<string> formatter = null, IPresenter<StringTextBinder, string> presenter = null)
        {
            if (_bindDict.ContainsKey(t)) return false;
            _bindDict.Add(t, new StringTextBinder(t, r, formatter, presenter).Bind());
            return true;
        }

        public bool Bind(TextMeshProUGUI t, IBoolRepository r, ITextFormatter<bool> formatter = null, IPresenter<BoolTextBinder, bool> presenter = null)
        {
            if (_bindDict.ContainsKey(t)) return false;
            _bindDict.Add(t, new BoolTextBinder(t, r, formatter, presenter).Bind());
            return true;
        }

        public bool Bind<V, R>(V v, IJsonRepository<R> r, IPresenter<JsonBinder<V, R>, R> presenter) where V : MonoBehaviour
        {
            if (_bindDict.ContainsKey(v)) return false;
            _bindDict.Add(v, new JsonBinder<V, R>(v, r, presenter).Bind());
            return true;
        }

        public bool Bind<V>(V v, IIntRepository r, IPresenter<FilledImageBinder, int> presenter = null) where V : Image
        {
            if (_bindDict.ContainsKey(v)) return false;
            _bindDict.Add(v, new FilledImageBinder(v, r, presenter).Bind());
            return true;
        }

        public bool Unbind(MonoBehaviour t)
        {
            if (!_bindDict.ContainsKey(t)) return false;
            _bindDict[t].Unbind();
            _bindDict.Remove(t);
            return true;
        }


        public bool BindActive(GameObject go, IBoolRepository repository, bool isReversed = false)
        {
            if (_activeBinderDict.ContainsKey(go))
            {
                this.LogError(SharedLogTag.UIBind, nameof(go), go.name, nameof(repository), repository.Name, "reason", "Already binded");
                return false;
            }

            _activeBinderDict.Add(go, new BoolGameObjectActiveBinder(go, repository, isReversed).Bind());
            return true;
        }

        public bool UnbindActive(GameObject go)
        {
            if (!_activeBinderDict.ContainsKey(go)) return false;
            _activeBinderDict[go].Unbind();
            _activeBinderDict.Remove(go);
            return true;
        }
    }
}