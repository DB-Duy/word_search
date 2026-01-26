using System;

namespace Shared.Core.IoC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RepositoryAttribute : BaseAttribute
    {
        public RepositoryAttribute(bool lazy = true) : base(lazy) { }
    }
}