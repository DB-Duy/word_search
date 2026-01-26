namespace Shared.Tracking.ErrorTracking
{
    public interface IValidator
    {
        bool Validate(string log);
    }
}