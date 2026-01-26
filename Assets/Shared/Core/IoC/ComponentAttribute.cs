using System;

namespace Shared.Core.IoC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ComponentAttribute : BaseAttribute
    {
        public ComponentAttribute(bool lazy = true) : base(lazy) { }
    }
}