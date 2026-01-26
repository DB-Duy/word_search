using Shared.Core.IoC;
using Shared.Core.Repository.BoolType;
using Shared.Core.Repository.IntType;
using Shared.Core.Repository.JsonType;
using Shared.Core.Repository.StringType;
using Shared.Core.View.Binding.Binder;
using Shared.Core.View.Binding.Container;
using Shared.Core.View.Binding.Formatter;
using Shared.Core.View.Binding.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.Core.View.Binding
{
    public static class UIBindExtensions
    {
        private static IBindContainer cachedContainer = null;
        private static IBindContainer Container => cachedContainer ??= IoCExtensions.Resolve<IBindContainer>();

        public static bool Bind(this ISharedUtility api, TextMeshProUGUI t, IIntRepository r, ITextFormatter<int> formatter = null, IPresenter<IntTextBinder, int> presenter = null)
            => Container.Bind(t, r, formatter, presenter);
        
        public static bool Bind(this ISharedUtility api, TextMeshProUGUI t, IStringRepository r, ITextFormatter<string> formatter = null, IPresenter<StringTextBinder, string> presenter = null)
            => Container.Bind(t, r, formatter, presenter);
        
        public static bool Bind(this ISharedUtility api, TextMeshProUGUI t, IBoolRepository r, ITextFormatter<bool> formatter = null, IPresenter<BoolTextBinder, bool> presenter = null)
            => Container.Bind(t, r, formatter, presenter);

        public static bool Bind<V, R>(this ISharedUtility api, V v, IJsonRepository<R> r, IPresenter<JsonBinder<V, R>, R> presenter) where V : MonoBehaviour
            => Container.Bind(v, r, presenter);
        
        public static bool Bind<V>(this ISharedUtility api, V v, IIntRepository r, IPresenter<FilledImageBinder, int> presenter = null) where V : Image
            => Container.Bind(v, r, presenter);

        public static bool Unbind(this ISharedUtility api, MonoBehaviour t)
            => Container.Unbind(t);
        
        public static bool BindActive(this ISharedUtility me, GameObject go, IBoolRepository repository, bool isReversed = false)
            => Container.BindActive(go, repository, isReversed);

        public static bool UnbindActive(this ISharedUtility me, GameObject go)
            => Container.UnbindActive(go);

        // -------------------------------------------------------------------------------------------------------------
        // More utility functions
        // -------------------------------------------------------------------------------------------------------------
        public static bool Bind<R>(this ISharedUtility api, TextMeshProUGUI t, ITextFormatter<int> formatter = null, IPresenter<IntTextBinder, int> presenter = null) where R : IIntRepository
            => Container.Bind(t, IoCExtensions.Resolve<R>(), formatter, presenter);
        
        public static bool Bind<R>(this ISharedUtility api, TextMeshProUGUI t, ITextFormatter<string> formatter = null, IPresenter<StringTextBinder, string> presenter = null) where R : IStringRepository 
            => Container.Bind(t, IoCExtensions.Resolve<R>(), formatter, presenter);
        
        public static bool Bind<R>(this ISharedUtility api, TextMeshProUGUI t, ITextFormatter<bool> formatter = null, IPresenter<BoolTextBinder, bool> presenter = null) where R : IBoolRepository
            => Container.Bind(t, IoCExtensions.Resolve<R>(), formatter, presenter);

        public static bool BindActive<T>(this ISharedUtility me, GameObject go , bool isReversed = false) where T : IBoolRepository
            => Container.BindActive(go, IoCExtensions.Resolve<T>(), isReversed);
    }
}