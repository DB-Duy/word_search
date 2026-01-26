namespace Shared.Core.Resolver
{
    public interface IResolver<R, F>
    {
        R Resolve(F from, R defaultValue = default);
    }
}