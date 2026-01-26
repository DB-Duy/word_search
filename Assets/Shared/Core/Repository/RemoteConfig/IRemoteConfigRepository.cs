namespace Shared.Core.Repository.RemoteConfig
{
    public interface IRemoteConfigRepository<T>
    {
        public string Name { get; }
        public T Get();
    }
}
