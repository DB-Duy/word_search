using System;
using Shared.Core.Repository.Default;

namespace Shared.Core.Repository.ObjectType
{
    public abstract class ObjectBaseRepository<T>: IObjectRepository<T>, IDefaultRepositoryExtensions 
    {
        public string Name { get; }
        public T DefaultValue { get; }
        public IObjectRepository<T>.OnValueUpdated onValueUpdated { get; } = new();

        protected ObjectBaseRepository(string name, T defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
            if (!string.IsNullOrEmpty(Name)) return;
            Name = this.GetRepositoryName(GetType());
            DefaultValue = this.GetRepositoryObject<T>(GetType());
            if (string.IsNullOrEmpty(Name)) throw new Exception($"string.IsNullOrEmpty(Name) for {GetType().FullName}");
        }

        public abstract T Get();
        public abstract void Save(T ob);
        public abstract void Save(object ob);
        public abstract bool IsExisted();
        public abstract void Delete();
    }
}