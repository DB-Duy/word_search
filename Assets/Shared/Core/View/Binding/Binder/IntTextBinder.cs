using Shared.Core.Repository.IntType;
using Shared.Core.View.Binding.Formatter;
using Shared.Core.View.Binding.Presenter;
using Shared.Utils;
using TMPro;

namespace Shared.Core.View.Binding.Binder
{
    public class IntTextBinder : IBinder
    {
        private const string Tag = "IntTextBinder";

        public TextMeshProUGUI TextLabel { get; private set; }
        public IIntRepository Repository { get; private set; }
        public ITextFormatter<int> TextFormatter { get; private set; }
        private IPresenter<IntTextBinder, int> Presenter { get; }

        public IntTextBinder(TextMeshProUGUI textLabel, IIntRepository repository, ITextFormatter<int> textFormatter = null, IPresenter<IntTextBinder, int> presenter = null)
        {
            TextLabel = textLabel;
            Repository = repository;
            TextFormatter = textFormatter;
            Presenter = presenter;
        }

        public IBinder Bind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Bind", nameof(TextLabel), TextLabel.name, nameof(Repository), Repository.Name, nameof(TextFormatter), TextFormatter == null ? "null" : TextFormatter.GetType().FullName);
            Repository.onIntValueUpdated.AddListener(_OnIntValueUpdated);
            var currentValue = Repository.Get();
            TextLabel.text = TextFormatter == null ? currentValue.ToString() : TextFormatter.Format(currentValue);
            return this;
        }

        public void Unbind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Unbind");
            Repository.onIntValueUpdated.RemoveListener(_OnIntValueUpdated);
        }

        private void _OnIntValueUpdated(int oldValue, int newValue)
        {
            if (Presenter == null)
                TextLabel.text = TextFormatter == null ? newValue.ToString() : TextFormatter.Format(newValue);
            else
                Presenter.Present(this, newValue);
        }
    }
}