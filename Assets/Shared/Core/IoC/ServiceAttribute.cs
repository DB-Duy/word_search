using System;

namespace Shared.Core.IoC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : BaseAttribute
    {
        public ServiceAttribute(bool lazy = true) : base(lazy) { }
    }
}