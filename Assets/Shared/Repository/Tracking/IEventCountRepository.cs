namespace Shared.Repository.Tracking
{
    public interface IEventCountRepository
    {
        int IncreaseAndGet(string countKey);
    }
}