using UnityEngine.Events;

namespace Shared.Core.Repository.IntType
{
    public interface IIntRepository
    {
        [System.Serializable]
        public class OnIntValueUpdated : UnityEvent<int, int> { }

        string Name { get; }
        int DefaultValue { get; }

        OnIntValueUpdated onIntValueUpdated { get; }

        int Get();

        void Set(int newValue);
        bool SetIfLargeThanCurrentValue(int newValue);

        int AddMore(int more);
        int Minus(int less);

        bool IsGreaterThanEqual(int val);
        bool IsGreaterThanZero();

        void Delete();
    }
}
