using UnityEngine.Events;

namespace Shared.Core.Repository.JsonType
{
    public interface IJsonRepository<T>
    {
        [System.Serializable]
        public class OnValueUpdated : UnityEvent<T> { }

        string Name { get; }
        T DefaultValue { get; }
        
        OnValueUpdated onValueUpdated { get; }
    
        T Get();
        void Save(object ob);
        
        bool IsExisted();
        void Delete();
    }
}
