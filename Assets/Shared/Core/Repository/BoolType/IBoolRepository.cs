using Shared.Core.Repository.BoolType.Handler;
using UnityEngine.Events;

namespace Shared.Core.Repository.BoolType
{
    public interface IBoolRepository
    {
        [System.Serializable]
        public class OnValueChanged : UnityEvent<bool> { };
        
        string Name { get; }
        bool DefaultValue { get; }

        OnValueChanged onValueChanged { get; }
        IBoolRepository AddHandlers(params IValueChangedHandler[] handlers);
        
        void Set(bool newValue);
        bool Get();
    }
}
