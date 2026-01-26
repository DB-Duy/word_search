namespace Shared.Service.Fps.Handlers
{
    public interface ISharedFpsHandler
    {
        void Handle(int framePerSecond);
    }
}