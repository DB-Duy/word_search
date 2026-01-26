using System;

namespace Shared.Utilities.SharedAction
{
    public class ConditionalAction : IConditionalAction
    {
        private readonly Func<bool> _validationAction;
        private readonly Action _executionAction;

        public ConditionalAction(Func<bool> validationAction, Action executionAction)
        {
            _validationAction = validationAction;
            _executionAction = executionAction;
        }

        public bool Execute()
        {
            if (!_validationAction.Invoke()) return false;
            _executionAction.Invoke();
            return true;
        }
    }
}