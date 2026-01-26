using Shared.Core.Async;

namespace Shared.Core.Validator.Async
{
    public interface IAsyncValidator
    {
        IAsyncOperation Validate();
    }
    
    public interface IAsyncValidator<T>
    {
        IAsyncOperation Validate(T t);
    }
}