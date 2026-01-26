using UnityEngine.Events;

namespace Shared.Core.Repository.StringType
{
    public interface IStringRepository
    {
        [System.Serializable]
        public class OnValueUpdated : UnityEvent<string, string> { }

        string Name { get; }
        string DefaultValue { get; }
        
        OnValueUpdated onValueUpdated { get; }

        string Get();
        bool Set(string diffValue);

        void Delete();
    }
}
