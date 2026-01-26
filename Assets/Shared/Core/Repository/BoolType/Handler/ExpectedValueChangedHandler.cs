using System;

namespace Shared.Core.Repository.BoolType.Handler
{
    public class ExpectedValueChangedHandler : IValueChangedHandler
    {
        private readonly bool _expectedValue;
        private readonly Action<bool> _action;

        public ExpectedValueChangedHandler(bool expectedValue, Action<bool> action)
        {
            _expectedValue = expectedValue;
            _action = action;
        }

        public void OnValueChanged(bool newValue)
        {
            if (_expectedValue == newValue) _action?.Invoke(newValue);
        }
    }
}