using Shared.Core.Repository.IntType;
using Shared.Core.View.Binding.Presenter;
using Shared.Utils;
using UnityEngine.UI;

namespace Shared.Core.View.Binding.Binder
{
    public class FilledImageBinder : IBinder
    {
        private const string Tag = "FilledImageBinder";

        public Image FilledImage { get; private set; }
        public IIntRepository Repository { get; private set; }
        private IPresenter<FilledImageBinder, int> Presenter { get; }

        public FilledImageBinder(Image filledImage, IIntRepository repository, IPresenter<FilledImageBinder, int> presenter = null)
        {
            FilledImage = filledImage;
            Repository = repository;
            Presenter = presenter;
        }

        public IBinder Bind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Bind", nameof(FilledImage), FilledImage.name, nameof(Repository), Repository.Name);
            Repository.onIntValueUpdated.AddListener(_OnIntValueUpdated);

            var currentValue = Repository.Get();
            Presenter.Present(this, currentValue);
            return this;
        }

        public void Unbind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Unbind");
            Repository.onIntValueUpdated.RemoveListener(_OnIntValueUpdated);
        }

        private void _OnIntValueUpdated(int oldValue, int newValue)
        {
            Presenter.Present(this, newValue);
        }
    }
}