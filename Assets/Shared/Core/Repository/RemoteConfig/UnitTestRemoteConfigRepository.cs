namespace Shared.Core.Repository.RemoteConfig
{
    public class UnitTestRemoteConfigRepository<T> : IRemoteConfigRepository<T>
    {
        private const string Tag = "UnitTestRemoteConfigRepository";

        public string Name { get; }
        private readonly T _defaultValue;

        public UnitTestRemoteConfigRepository(string name, T defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new System.Exception($"{Tag}->Constructor: Invalid name={name}");
            Name = name;
            _defaultValue = defaultValue;
        }

        public T Get() => _defaultValue;
    }
}