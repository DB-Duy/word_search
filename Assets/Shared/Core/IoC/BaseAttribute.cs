using System;

namespace Shared.Core.IoC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public abstract class BaseAttribute : Attribute
    {
        public bool Lazy { get; }

        protected BaseAttribute(bool lazy = true)
        {
            Lazy = lazy;
        }
    }
}