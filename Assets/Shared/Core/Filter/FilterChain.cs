using System.Linq;

namespace Shared.Core.Filter
{
    public class FilterChain : IFilter
    {
        private readonly IFilter[] _filters;


        public FilterChain(params IFilter[] filters)
        {
            _filters = filters;
        }

        public bool Filter() => _filters.All(v => v.Filter());
    }
    
    public class FilterChain<T> : IFilter<T>
    {
        private readonly IFilter<T>[] _filters;


        public FilterChain(params IFilter<T>[] filters)
        {
            _filters = filters;
        }

        public bool Filter(T t) => _filters.All(v => v.Filter(t));
    }
    
    public class FilterChain<T, U> : IFilter<T, U>
    {
        private readonly IFilter<T, U>[] _filters;


        public FilterChain(IFilter<T, U>[] filters)
        {
            _filters = filters;
        }

        public bool Filter(T t, U u) => _filters.All(v => v.Filter(t, u));
    }
}