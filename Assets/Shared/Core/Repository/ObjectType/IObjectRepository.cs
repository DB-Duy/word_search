using UnityEngine.Events;

namespace Shared.Core.Repository.ObjectType
{
    public interface IObjectRepository<T>
    {
        [System.Serializable]
        public class OnValueUpdated : UnityEvent<T, T>
        {
        }

        string Name { get; }
        T DefaultValue { get; }

        OnValueUpdated onValueUpdated { get; }

        T Get();
        void Save(T ob);

        bool IsExisted();
        void Delete();
    }
}