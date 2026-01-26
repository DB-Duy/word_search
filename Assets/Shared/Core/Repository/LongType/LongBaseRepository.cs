using System;
using Shared.Core.Repository.Default;

namespace Shared.Core.Repository.LongType
{
    public abstract class LongBaseRepository : ILongRepository, IDefaultRepositoryExtensions
    {
        public string Name { get; private set; }
        public long DefaultValue { get; private set; }
        public ILongRepository.OnValueUpdated onValueUpdated { get; } = new();

        protected LongBaseRepository(string name = null, long defaultValue = 0)
        {
            Name = name;
            DefaultValue = defaultValue;
            if (!string.IsNullOrEmpty(Name)) return;
            Name = this.GetRepositoryName(GetType());
            DefaultValue = this.GetRepositoryLong(GetType());
            if (string.IsNullOrEmpty(Name)) throw new Exception($"string.IsNullOrEmpty(Name) for {GetType().FullName}");
        }

        public abstract long Get();

        public abstract void Set(long newValue);

        public bool SetIfLargeThanCurrentValue(long newValue)
        {
            var oldValue = Get();
            if (newValue <= oldValue) return false;
            Set(newValue);
            return true;
        }

        public long AddMore(long more)
        {
            if (more == 0) return Get();
            var oldValue = Get();
            var newValue = oldValue + more;
            Set(newValue);
            return newValue;
        }

        public long Minus(long less)
        {
            if (less == 0) return Get();
            var oldValue = Get();
            var newValue = oldValue - less;
            Set(newValue);
            return newValue;
        }

        public bool IsGreaterThanEqual(long val) => Get() >= val;

        public abstract void Delete();
    }
}