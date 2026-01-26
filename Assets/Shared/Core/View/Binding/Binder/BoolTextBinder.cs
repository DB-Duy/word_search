using Shared.Core.Repository.BoolType;
using Shared.Core.View.Binding.Formatter;
using Shared.Core.View.Binding.Presenter;
using Shared.Utils;
using TMPro;

namespace Shared.Core.View.Binding.Binder
{
    public class BoolTextBinder : IBinder
    {
        private const string Tag = "BoolTextBinder";

        public TextMeshProUGUI TextLabel { get; private set; }
        public IBoolRepository Repository { get; private set; }
        public ITextFormatter<bool> TextFormatter { get; private set; }
        private IPresenter<BoolTextBinder, bool> Presenter { get; }

        public BoolTextBinder(TextMeshProUGUI textLabel, IBoolRepository repository, ITextFormatter<bool> textFormatter = null, IPresenter<BoolTextBinder, bool> presenter = null)
        {
            TextLabel = textLabel;
            Repository = repository;
            TextFormatter = textFormatter;
            Presenter = presenter;
        }

        public IBinder Bind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Bind", nameof(TextLabel), TextLabel.name, nameof(Repository), Repository.Name, nameof(TextFormatter), TextFormatter == null ? "null" : TextFormatter.GetType().FullName);
            Repository.onValueChanged.AddListener(_OnValueChanged);

            var currentValue = Repository.Get();
            TextLabel.text = TextFormatter == null ? currentValue.ToString() : TextFormatter.Format(currentValue);
            return this;
        }

        public void Unbind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Unbind");
            Repository.onValueChanged.RemoveListener(_OnValueChanged);
        }

        private void _OnValueChanged(bool newValue)
        {
            if (Presenter == null) 
                TextLabel.text = TextFormatter == null ? newValue.ToString() : TextFormatter.Format(newValue);
            else 
                Presenter.Present(this, newValue);
        }
    }
}