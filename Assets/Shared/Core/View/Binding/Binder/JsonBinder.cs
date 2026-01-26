using Shared.Core.Repository.JsonType;
using Shared.Core.View.Binding.Presenter;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.View.Binding.Binder
{
    public class JsonBinder<V, R> : IBinder where V : MonoBehaviour
    {
        private const string Tag = "JsonBinder";

        public V View { get; private set; }
        public IJsonRepository<R> Repository { get; private set; }
        private IPresenter<JsonBinder<V, R>, R> Presenter { get; }

        public JsonBinder(V view, IJsonRepository<R> repository, IPresenter<JsonBinder<V, R>, R> presenter)
        {
            View = view;
            Repository = repository;
            Presenter = presenter;
        }

        public IBinder Bind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Bind", nameof(View), View.name, nameof(Repository), Repository.Name);
            Repository.onValueUpdated.AddListener(_OnValueUpdated);

            var currentValue = Repository.Get();
            Presenter.Present(this, currentValue);
            return this;
        }

        public void Unbind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Unbind");
            Repository.onValueUpdated.RemoveListener(_OnValueUpdated);
        }

        private void _OnValueUpdated(R newValue)
        {
            Presenter.Present(this, newValue);
        }
    }
}