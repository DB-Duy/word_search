namespace Shared.Core.Filter
{
    public interface IFilter
    {
        bool Filter();
    }
    
    public interface IFilter<T>
    {
        bool Filter(T t);
    }
    
    public interface IFilter<T, U>
    {
        bool Filter(T t, U u);
    }
}