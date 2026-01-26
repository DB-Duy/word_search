using System;
using Shared.Core.Repository.Default;

namespace Shared.Core.Repository.StringType
{
    public abstract class StringBaseRepository : IStringRepository, IDefaultRepositoryExtensions
    {
        public string Name { get; }
        public string DefaultValue { get; }
        public IStringRepository.OnValueUpdated onValueUpdated { get; } = new();

        protected StringBaseRepository(string name = null, string defaultValue = null)
        {
            Name = name;
            DefaultValue = defaultValue;
            if (!string.IsNullOrEmpty(Name)) return;
            Name = this.GetRepositoryName(GetType());
            DefaultValue = this.GetRepositoryString(GetType());
            if (string.IsNullOrEmpty(Name)) throw new Exception($"string.IsNullOrEmpty(Name) for {GetType().FullName}");
        }

        public abstract string Get();

        public abstract bool Set(string diffValue);

        public abstract void Delete();
    }
}