using Shared.Core.Repository.StringType;
using Shared.Core.View.Binding.Formatter;
using Shared.Core.View.Binding.Presenter;
using Shared.Utils;
using TMPro;

namespace Shared.Core.View.Binding.Binder
{
    public class StringTextBinder : IBinder
    {
        private const string Tag = "StringTextBinder";

        public TextMeshProUGUI TextLabel { get; private set; }
        public IStringRepository Repository { get; private set; }
        public ITextFormatter<string> TextFormatter { get; private set; }
        private IPresenter<StringTextBinder, string> Presenter { get; }

        public StringTextBinder(TextMeshProUGUI textLabel, IStringRepository repository, ITextFormatter<string> textFormatter = null, IPresenter<StringTextBinder, string> presenter = null)
        {
            TextLabel = textLabel;
            Repository = repository;
            TextFormatter = textFormatter;
            Presenter = presenter;
        }

        public IBinder Bind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Bind", nameof(TextLabel), TextLabel.name, nameof(Repository), Repository.Name, nameof(TextFormatter), TextFormatter == null ? "null" : TextFormatter.GetType().FullName);
            Repository.onValueUpdated.AddListener(_OnValueUpdated);
            
            var currentValue = Repository.Get();
            TextLabel.text = TextFormatter == null ? currentValue : TextFormatter.Format(currentValue);
            return this;
        }

        public void Unbind()
        {
            SharedLogger.LogJson(SharedLogTag.UIBind, $"{Tag}->Unbind");
            Repository.onValueUpdated.RemoveListener(_OnValueUpdated);
        }

        private void _OnValueUpdated(string oldValue, string newValue)
        {
            if (Presenter == null)
                TextLabel.text = TextFormatter == null ? newValue : TextFormatter.Format(newValue);
            else
                Presenter.Present(this, newValue);
        }
    }
}