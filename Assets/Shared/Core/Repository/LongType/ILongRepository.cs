using UnityEngine.Events;

namespace Shared.Core.Repository.LongType
{
    public interface ILongRepository
    {
        [System.Serializable]
        public class OnValueUpdated : UnityEvent<long, long> { }

        string Name { get; }
        long DefaultValue { get; }

        OnValueUpdated onValueUpdated { get; }

        long Get();

        void Set(long newValue);
        bool SetIfLargeThanCurrentValue(long newValue);

        long AddMore(long more);
        long Minus(long less);

        bool IsGreaterThanEqual(long val);

        void Delete();
    }
}
