using System;

public interface IIntervalValidator
{
    void Mark();
    bool Validate();
    bool Validate(float intervalInSeconds);
}

public class IntervalValidator : IIntervalValidator
{
    private DateTime _startDateTime = DateTime.MinValue;
    private float _requiredIntervalInSeconds = 0f;

    public IntervalValidator(float requiredIntervalInSeconds)
    {
        _requiredIntervalInSeconds = requiredIntervalInSeconds;
    }

    public void Mark()
    {
        _startDateTime = DateTime.Now;
    }

    public bool Validate()
    {
        return (DateTime.Now - _startDateTime).TotalSeconds >= _requiredIntervalInSeconds;
    }

    public bool Validate(float intervalInSeconds)
    {
        return (DateTime.Now - _startDateTime).TotalSeconds >= intervalInSeconds;
    }
}
