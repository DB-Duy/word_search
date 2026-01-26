namespace Shared.Core.Validator
{
    public interface IValidator
    {
        bool Validate();
    }
    
    public interface IValidator<T>
    {
        bool Validate(T t);
    }
    
    public interface IValidator<T, U>
    {
        bool Validate(T t, U u);
    }
}