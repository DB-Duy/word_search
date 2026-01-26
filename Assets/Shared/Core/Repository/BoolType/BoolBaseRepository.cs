using System;
using System.Collections.Generic;
using Shared.Core.Repository.BoolType.Handler;
using Shared.Core.Repository.Default;
using Shared.Utils;

namespace Shared.Core.Repository.BoolType
{
    public abstract class BoolBaseRepository : IBoolRepository, IDefaultRepositoryExtensions
    {
        public string Name { get; protected set; }
        public bool DefaultValue { get; protected set; }

        public IBoolRepository.OnValueChanged onValueChanged { get; } = new();
        protected readonly HashSet<IValueChangedHandler> Handlers = new();
        
        protected BoolBaseRepository(string name = null, bool defaultValue = false)
        {
            Name = name;
            DefaultValue = defaultValue;
            if (!string.IsNullOrEmpty(Name)) return;
            Name = this.GetRepositoryName(GetType());
            DefaultValue = this.GetRepositoryBool(GetType());
            if (string.IsNullOrEmpty(Name)) throw new Exception($"string.IsNullOrEmpty(Name) for {GetType().FullName}");
        }

        public IBoolRepository AddHandlers(params IValueChangedHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return this;
        }

        public abstract void Set(bool newValue);

        public abstract bool Get();
    }
}