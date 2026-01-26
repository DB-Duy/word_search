using System;

namespace Shared.Core.IoC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SignalEventAttribute : BaseAttribute
    {
        public SignalEventAttribute(bool lazy = true) : base(lazy) { }
    }
}