using System;

namespace Shared.Core.Repository.BoolType.Handler
{
    public class ActionValueChangedHandler : IValueChangedHandler
    {
        private readonly Action<bool> _action;

        public ActionValueChangedHandler(Action<bool> action)
        {
            _action = action;
        }

        public void OnValueChanged(bool newValue)
        {
            _action?.Invoke(newValue);
        }
    }
}