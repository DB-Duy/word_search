using Shared.Core.Repository.BoolType;
using Shared.Core.Repository.IntType;
using Shared.Core.Repository.JsonType;
using Shared.Core.Repository.StringType;
using Shared.Core.View.Binding.Binder;
using Shared.Core.View.Binding.Formatter;
using Shared.Core.View.Binding.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.Core.View.Binding.Container
{
    public interface IBindContainer
    {
        bool Bind(TextMeshProUGUI t, IIntRepository r, ITextFormatter<int> formatter = null, IPresenter<IntTextBinder, int> presenter = null);
        bool Bind(TextMeshProUGUI t, IStringRepository r, ITextFormatter<string> formatter = null, IPresenter<StringTextBinder, string> presenter = null);
        bool Bind(TextMeshProUGUI t, IBoolRepository r, ITextFormatter<bool> formatter = null, IPresenter<BoolTextBinder, bool> presenter = null);
        
        bool Bind<V, R>(V v, IJsonRepository<R> r, IPresenter<JsonBinder<V, R>, R> presenter) where V : MonoBehaviour;
        bool Bind<V>(V v, IIntRepository r, IPresenter<FilledImageBinder, int> presenter = null) where V : Image;
        
        bool Unbind(MonoBehaviour t);
        
        bool BindActive(GameObject go, IBoolRepository repository, bool isReversed = false);
        bool UnbindActive(GameObject go);
    }
}