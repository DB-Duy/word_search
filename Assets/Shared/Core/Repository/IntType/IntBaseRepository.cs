using System;
using Shared.Core.Repository.Default;

namespace Shared.Core.Repository.IntType
{
    public abstract class IntBaseRepository : IIntRepository, IDefaultRepositoryExtensions
    {
        private const int Zero = 0;
        
        public string Name { get; }
        public int DefaultValue { get; }

        public IIntRepository.OnIntValueUpdated onIntValueUpdated { get; } = new();
        
        protected IntBaseRepository(string name = null, int defaultValue = 0)
        {
            Name = name;
            DefaultValue = defaultValue;
            if (!string.IsNullOrEmpty(Name)) return;
            Name = this.GetRepositoryName(GetType());
            DefaultValue = this.GetRepositoryInt(GetType());
            if (string.IsNullOrEmpty(Name)) throw new Exception($"string.IsNullOrEmpty(Name) for {GetType().FullName}");
        }

        public abstract int Get();

        public abstract void Set(int newValue);

        public bool SetIfLargeThanCurrentValue(int newValue)
        {
            var oldValue = Get();
            if (newValue <= oldValue) return false;
            Set(newValue);
            return true;
        }

        public int AddMore(int more)
        {
            if (more == 0) return Get();
            var oldValue = Get();
            var newValue = oldValue + more;
            Set(newValue);
            return newValue;
        }

        public int Minus(int less)
        {
            if (less == 0) return Get();
            var oldValue = Get();
            var newValue = oldValue - less;
            Set(newValue);
            return newValue;
        }

        public bool IsGreaterThanEqual(int val) => Get() >= val;

        public bool IsGreaterThanZero() => Get() > Zero;

        public abstract void Delete();
    }
}